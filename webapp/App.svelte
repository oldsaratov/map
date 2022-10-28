<script>

    import L from 'leaflet';
    import MarkerPopup from './MarkerPopup.svelte';
    import Toolbar from './Toolbar.svelte';
    import {Yandex} from "./Yandex";
    import * as markerIcons from './markers.js';
    import Sidebar from "./Sidebar.svelte";

    let map;
    let markerLayers;
    let selectedPhoto;
    let sidebarMode = false;

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
            fillMap();
        });

        return m;
    }

    function bindPopup(marker, createFn) {
        let popupComponent;
        marker.bindPopup(() => {
            let container = L.DomUtil.create('div');
            popupComponent = createFn(container);
            return container;
        });

        marker.on('popupclose', () => {
            if (popupComponent) {
                let old = popupComponent;
                popupComponent = null;
                // Wait to destroy until after the fadeout completes.
                setTimeout(() => {
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

    function arrowIcon(feature) {
        const rotationClassName = feature.properties.rotation ? 'rotation-' + feature.properties.rotation : "";
        const periodClassName = 'period-' + feature.properties.period;

        let html = `
            <div class="${rotationClassName} ${periodClassName}">
                ${feature.properties.rotation != null ? markerIcons.arrow : markerIcons.empty}
            </div>`;

        return L.divIcon({
            html,
            iconSize: [15, 15],
            className: 'map-marker'
        });
    }

    function createMarker(feature) {
        let icon = feature.properties.count === 1 ? arrowIcon(feature) : clusterIcon(feature);
        let marker = L.marker([feature.geometry.coordinates[1], feature.geometry.coordinates[0]], {icon});

        if (feature.properties.count > 1) {
            marker.on('click', (e) => {
                map.panTo(e.target.getLatLng());
                setTimeout(() => {
                    map.zoomIn();
                }, 500);
            });
        } else {
            if (sidebarMode) {
                marker.on('click', (e) => {
                    selectedPhoto = feature.properties.items[0];
                });
            } else {
                bindPopup(marker, (m) => {
                    selectedPhoto = feature.properties.items[0];
                    
                    return new MarkerPopup({
                        target: m,
                        props: {
                            item: feature.properties.items[0]
                        }
                    });
                });
            }
        }

        return marker;
    }

    function onRangeChanged(e) {
        period = e.detail.bound;
        if (map) {
            fillMap();
        }
    }
    
    function onSidebarClosed(e) {
        sidebarMode = false;
        selectedPhoto = null;
        
        if (map) {
            fillMap();
        }
    }

    function onSidebarStateChanged(e) {
        sidebarMode = e.detail.active;

        if (!sidebarMode) {
            selectedPhoto = null;
        }

        if (map) {
            map.invalidateSize(true);
            fillMap();
        }
    }

    function fillMap() {
        if (!map) {
            return;
        }

        const bbox = map.getBounds();

        if (markerLayers) {
            markerLayers.clearLayers()
        }

        markerLayers = L.layerGroup();

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
                const geoJson = L.geoJSON(data, {
                    pointToLayer: createMarker
                });

                markerLayers.addLayer(geoJson);
            });

        markerLayers.addTo(map);
    }

    function init(container) {
        map = createMap(container);

        fillMap();

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
        transform: rotate(0deg);
    }

    :global(.rotation-45) {
        transform: rotate(45deg);
    }

    :global(.rotation-90) {
        transform: rotate(90deg);
    }

    :global(.rotation-135) {
        transform: rotate(135deg);
    }

    :global(.rotation-180) {
        transform: rotate(180deg);
    }

    :global(.rotation-225) {
        transform: rotate(225deg);
    }

    :global(.rotation-270) {
        transform: rotate(270deg);
    }

    :global(.rotation-315) {
        transform: rotate(315deg);
    }

    :global(.period-0 .marker-color) {
        fill: #bebebe;
    }

    :global(.period-1 .marker-color) {
        fill: #710a0f;
    }

    :global(.period-2 .marker-color) {
        fill: #b3191c;
    }

    :global(.period-3 .marker-color) {
        fill: #c1381f;
    }

    :global(.period-4 .marker-color) {
        fill: #dd462b;
    }

    :global(.period-5 .marker-color) {
        fill: #e57b37;
    }

    :global(.period-6 .marker-color) {
        fill: #d8ab3d;
    }

    :global(.period-7 .marker-color) {
        fill: #cbda42;
    }

    :global(.period-8 .marker-color) {
        fill: #a0d034;
    }

    :global(.period-9 .marker-color) {
        fill: #75c626;
    }

    :global(.period-10 .marker-color) {
        fill: #49BC17;
    }

    .main {
        height: 100%;
    }

    .map-container {
        display: flex;
        height: 100%;
    }

    .map {
        flex-grow: 1;
        order: 1;
    }
</style>

<link rel="stylesheet" href="https://unpkg.com/leaflet@1.6.0/dist/leaflet.css"
      integrity="sha512-xwE/Az9zrjBIphAcBb3F6JVqxf46+CDLwfLMHloNu6KEQCAWi6HcDUbeOfBIptF7tcCzusKFjFw2yuvEpDL9wQ=="
      crossorigin=""/>
<div class="main">

    <Toolbar on:rangeChanged={onRangeChanged} on:sidebarStateChanged={onSidebarStateChanged} bind:sidebarMode/>

    <div class="map-container">
        <div id="map" class="map flex-element" use:init></div>

        {#if sidebarMode}
            <Sidebar bind:selectedPhoto on:sidebarClosed={onSidebarClosed} />
        {/if}

    </div>

</div>
