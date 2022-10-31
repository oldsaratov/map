<script>

    import L from 'leaflet';
    import MarkerPopup from './MarkerPopup.svelte';
    import Toolbar from './Toolbar.svelte';
    import {Yandex} from "./Yandex";
    import * as markerIcons from './markers.js';
    import Sidebar from "./Sidebar.svelte";

    let map;
    let markerLayers;
    let currentFeature;
    let currentMarker;
    let sidebarActive = false;

    const markersMap = new Map();

    const initialView = [51.5268, 46.0001];
    let period = [1863, new Date().getFullYear()];

    const accessToken = "pk.eyJ1Ijoib2tvbG9iYXhhIiwiYSI6Imt0RUVsVUkifQ.DjDf-hCRChe7FkfvguDmfw";

    function createMap(container) {
        let mapboxLayer = L.tileLayer(
            `//api.mapbox.com/styles/v1/okolobaxa/ckf6mp05x0ce519s9dotul5a3/tiles/{z}/{x}/{y}?access_token=${accessToken}`,
            {
                tileSize: 512,
                zoomOffset: -1,
                minZoom: 10,
                maxZoom: 20,
            }
        );

        let yandexSatellite = new Yandex('yandex#hybrid')

        let m = L.map(container, {
            preferCanvas: true,
            layers: [mapboxLayer]
        }).setView(initialView, 14);

        let baseLayers = {
            'Карта ФСС': mapboxLayer,
            'Яндекс.Карта (спутник)': yandexSatellite
        };
        L.control.layers(baseLayers).addTo(m);

        m.on('moveend', function () {
            updateMap();
        });

        markerLayers = L.layerGroup();
        markerLayers.addTo(m);

        return m;
    }

    function bindPopup(marker, createFn) {
        let popupComponent;
        marker.bindPopup(() => {
            let container = L.DomUtil.create('div');
            popupComponent = createFn(container);
            return container;
        }).openPopup();

        marker.on('popupclose', () => {
            if (!sidebarActive) {
                setCurrent(null, null);
            }
            
            if (popupComponent) {
                let old = popupComponent;
                popupComponent = null;
                // Wait to destroy until after the fadeout completes.
                setTimeout(() => {
                    marker.unbindPopup();
                    old.$destroy();
                }, 500);
            }
        });
    }

    function clusterIcon(feature) {
        const count = feature.properties.count;
        const digits = (count + '').length;

        return L.divIcon({
            html: count,
            className: 'cluster digits-' + digits,
            iconSize: null
        });
    }

    function arrowIcon(feature, isCurrent) {
        const rotationClassName = feature.properties.rotation ? 'rotation-' + feature.properties.rotation : "";
        const periodClassName = 'period-' + feature.properties.period;
        const rippleClassName = isCurrent ? 'ripple' : '';

        let html = `
                <div class="${rotationClassName} ${periodClassName}">
                    ${feature.properties.rotation != null ? markerIcons.arrow : markerIcons.empty}
                    <div class="${periodClassName} ${rippleClassName}"></div>
                </div>
            `;

        return L.divIcon({
            html,
            iconSize: [15, 15],
            className: 'map-marker',
            iconAnchor: [7.5, 10],
            popupAnchor: [1, -1]
        });
    }

    function setCurrent(marker, feature) {
        if (currentMarker && currentFeature) {
            currentMarker.setIcon(arrowIcon(currentFeature, false));
        }

        currentMarker = marker;
        currentFeature = feature;

        if (currentMarker && currentFeature) {
            currentMarker.setIcon(arrowIcon(currentFeature, true));
        }
    }

    function createMarker(feature) {
        let icon = feature.properties.count === 1 ? arrowIcon(feature, false) : clusterIcon(feature);
        let marker = L.marker([feature.geometry.coordinates[1], feature.geometry.coordinates[0]], {icon});

        marker.on('click', (e) => {
            if (feature.properties.count > 1) {
                map.panTo(e.target.getLatLng());
                setTimeout(() => {
                    map.zoomIn();
                }, 500);
            } else {
                setCurrent(e.target, feature);

                if (!sidebarActive) {
                    bindPopup(marker, (m) => {
                        return new MarkerPopup({
                            target: m,
                            props: {
                                item: feature.properties.items[0]
                            }
                        });
                    });
                }
            }
        });

        return marker;
    }

    function onRangeChanged(e) {
        period = e.detail.bound;
        if (map) {
            updateMap();
        }
    }

    function onDisplayModeChanged(e) {
        sidebarActive = e.detail.active;
        if (!sidebarActive) {
            setCurrent(null, null);
        } else {
            if (map) {
                if (currentMarker) {
                    map.panTo(currentMarker.getLatLng());
                    setCurrent(currentMarker, currentFeature);
                } else {
                    if (currentFeature) {
                        map.panTo([currentFeature.geometry.coordinates[1], currentFeature.geometry.coordinates[0]]);
                        setTimeout(() => {
                            if (!currentMarker) {
                                currentMarker = markersMap.get(currentFeature.properties.key);
                            }

                            setCurrent(currentMarker, currentFeature);
                        }, 500);
                    }
                }
            }
        }

        if (map) {
            map.closePopup();
            updateMap();
        }
    }

    function updateMap() {
        if (!map) {
            return;
        }

        const bbox = map.getBounds();

        fetch('/api/map?' + new URLSearchParams({
            east: bbox.getEast(),
            north: bbox.getNorth(),
            west: bbox.getWest(),
            south: bbox.getSouth(),
            zoom: map.getZoom(),
            from: period[0],
            to: period[1],
        }))
            .then((response) => response.json())
            .then((data) => {
                if (data.features) {
                    const actualMarkerIds = new Set();

                    data.features.forEach(feature => {
                        const key = feature.properties.key;
                        if (!markersMap.has(key)) {
                            const marker = createMarker(feature);

                            markersMap.set(key, marker);

                            markerLayers.addLayer(marker);
                        }

                        actualMarkerIds.add(key);
                    });

                    for (const [key, marker] of markersMap) {
                        if (!actualMarkerIds.has(key)) {
                            markerLayers.removeLayer(marker)
                            markersMap.delete(key);
                        }
                    }
                }
            });
    }

    function init(container) {
        map = createMap(container);

        updateMap();

        return {
            destroy: () => {
                map.remove();
                map = null;
            }
        };
    }

</script>

<style>
    :global(.cluster) {
        background: #2d84c8;
        border-radius: 50%;
        text-align: center;
        color: white;
        font-weight: 700;
        border: 1px solid #2d84c8;
        font-family: monospace;
    }

    :global(.cluster:before) {
        content: ' ';
        position: absolute;
        border-radius: 50%;
        z-index: -1;
        top: 1px;
        left: 1px;
        right: 1px;
        bottom: 1px;
        border: 1px solid white;
    }

    :global(.digits-1) {
        font-size: 14px;
        height: 28px;
        width: 28px;
        line-height: 28px;
        margin-top: -14px;
        margin-left: -14px;
    }

    :global(.digits-2) {
        font-size: 16px;
        height: 34px;
        width: 34px;
        line-height: 35px;
        margin-top: -17px;
        margin-left: -17px;
    }

    :global(.digits-2:before) {
        border-width: 2px;
    }

    :global(.digits-3) {
        font-size: 18px;
        height: 48px;
        width: 47px;
        line-height: 47px;
        border-width: 3px;
        margin-top: -24px;
        margin-left: -24px;
    }

    :global(.digits-3:before) {
        border-width: 3px;
    }

    :global(.digits-4) {
        font-size: 18px;
        height: 58px;
        width: 58px;
        line-height: 57px;
        border-width: 4px;
        margin-top: -29px;
        margin-left: -29px;
    }

    :global(.digits-4:before) {
        border-width: 4px;
    }

    :global(.digits-5) {
        font-size: 18px;
        height: 78px;
        width: 78px;
        line-height: 78px;
        border-width: 4px;
        margin-top: -39px;
        margin-left: -39px;
    }

    :global(.digits-5:before) {
        border-width: 5px;
    }

    :global(.rotation-0) {
        transform-origin: center;
        transform: rotate(360deg);
    }

    :global(.rotation-45) {
        transform-origin: center;
        transform: rotate(45deg);
    }

    :global(.rotation-90) {
        transform-origin: center;
        transform: rotate(90deg);
    }

    :global(.rotation-135) {
        transform-origin: center;
        transform: rotate(135deg);
    }

    :global(.rotation-180) {
        transform-origin: center;
        transform: rotate(180deg);
    }

    :global(.rotation-225) {
        transform-origin: center;
        transform: rotate(225deg);
    }

    :global(.rotation-270) {
        transform-origin: center;
        transform: rotate(270deg);
    }

    :global(.rotation-315) {
        transform-origin: center;
        transform: rotate(315deg);
    }

    :global(.period-0 .marker-color, .period-0.ripple) {
        fill: #bebebe;
        border-color: #bebebe;
    }

    :global(.period-1 .marker-color, .period-1.ripple) {
        fill: #710a0f;
        border-color: #710a0f;
    }

    :global(.period-2 .marker-color, .period-2.ripple) {
        fill: #b3191c;
        border-color: #b3191c;
    }

    :global(.period-3 .marker-color, .period-3.ripple) {
        fill: #c1381f;
        border-color: #c1381f;
    }

    :global(.period-4 .marker-color, .period-4.ripple) {
        fill: #dd462b;
        border-color: #dd462b;
    }

    :global(.period-5 .marker-color, .period-5.ripple) {
        fill: #e57b37;
        border-color: #e57b37;
    }

    :global(.period-6 .marker-color, .period-6.ripple) {
        fill: #d8ab3d;
        border-color: #d8ab3d;
    }

    :global(.period-7 .marker-color, .period-7.ripple) {
        fill: #cbda42;
        border-color: #cbda42;
    }

    :global(.period-8 .marker-color, .period-8.ripple) {
        fill: #a0d034;
        border-color: #a0d034;
    }

    :global(.period-9 .marker-color, .period-9.ripple) {
        fill: #75c626;
        border-color: #75c626;
    }

    :global(.period-10 .marker-color, .period-10.ripple) {
        fill: #49BC17;
        border-color: #49BC17;
    }

    :global(.arrow-icon) {
        fill: white;
    }

    .main {
        height: 100%;
    }

    .map-container {
        display: flex;
        height: calc(100vh - 100px);
    }

    .map {
        flex-grow: 1;
        order: 1;
    }

    :global(.ripple) {
        border-width: 3px;
        border-style: solid;
        border-radius: 75px;
        height: 75px;
        width: 75px;
        animation: pulsate 2s ease-out;
        animation-iteration-count: infinite;
        position: absolute;
        bottom: -30px;
        right: -34px;
    }

    @-webkit-keyframes pulsate {
        0% {
            transform: scale(0.1, 0.1);
            opacity: 0.0;
        }
        50% {
            opacity: 1.0;
        }
        100% {
            transform: scale(1.2, 1.2);
            opacity: 0.0;
        }
    }
</style>

<link rel="stylesheet" href="https://unpkg.com/leaflet@1.6.0/dist/leaflet.css"
      integrity="sha512-xwE/Az9zrjBIphAcBb3F6JVqxf46+CDLwfLMHloNu6KEQCAWi6HcDUbeOfBIptF7tcCzusKFjFw2yuvEpDL9wQ=="
      crossorigin=""/>
<div class="main">

    <Toolbar on:rangeChanged={onRangeChanged} on:displayModeChanged={onDisplayModeChanged} bind:sidebarActive/>

    <div class="map-container">
        <div id="map" class="map flex-element" use:init></div>

        {#if sidebarActive}
            <Sidebar bind:currentFeature on:sidebarClosed={onDisplayModeChanged}/>
        {/if}

    </div>

</div>
