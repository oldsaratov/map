<script>
    import Period from "./Period.svelte";
    import {createEventDispatcher} from 'svelte';
    import { LottiePlayer } from '@lottiefiles/svelte-lottie-player';

    const dispatch = createEventDispatcher();
    export let currentFeature;
    let details;

    $:currentFeature && getDetails();

    function getDetails() {
        if (currentFeature) {
            fetch(`https://oldsaratov.ru/api/node/${currentFeature.properties.item.id}/`)
                .then((response) => response.json())
                .then((data) => {
                    details = data;
                });
        }
    }

    function onSidebarClose(e) {
        dispatch('sidebarClosed', {
            active: false
        });
    }

</script>

<div class="sidebar">
    <div class="close"><span on:click={onSidebarClose}>×</span></div>
    {#if currentFeature}
        <div class="content-container">
            <div class="item-image">
                <h4>{currentFeature.properties.item.title}</h4>
                <a target="_blank" href="{currentFeature.properties.item.url}">
                    <img src="{currentFeature.properties.item.photoUrl}" alt="{currentFeature.properties.item.title}">
                </a>
            </div>
            <div class="item-period">
                <Period bind:periodFrom={currentFeature.properties.item.periodFrom} bind:periodTo={currentFeature.properties.item.periodTo}/>
            </div>
            <div class="item-description">
                {#if details && details.body && details.body.und}
                    <div class="truncate">
                        {@html details.body.und[0].value}
                    </div>
                {/if}
            </div>
            <div class="item-footer">
                <a class="button" target="_blank" href="{currentFeature.properties.item.url}">Открыть ❯</a>
            </div>
        </div>
    {:else}
        <div class="empty-container">
            <p class="empty-text">Нажимите на маркер фотографии для просмотра подробностей</p>
            <div class="empty-animation">
                <LottiePlayer
                        src="https://assets9.lottiefiles.com/private_files/lf30_noclpt6t.json"
                        autoplay="{true}"
                        loop="{true}"
                        controls="{false}"
                        renderer="svg"
                        background="transparent"
                        height="{350}"
                        width="{350}"
                />
            </div>
        </div>
    {/if}
</div>

<style>
    .sidebar {
        position: absolute;
        top: 100px;
        left: 0;
        background: white;
        width: 400px;
        height: calc(100vh - 100px);
        display: flex;
        border-bottom-right-radius: 8px;
        border-top-right-radius: 8px;
        box-shadow: 0 3px 14px rgb(0 0 0 / 40%);
        z-index: 9999;
    }

    .content-container {
        padding-left: 20px;
        padding-right: 20px;
        display: flex;
        flex-direction: column;
    }
    
    .item-image img {
        max-width: 350px;
        min-width: 350px;
    }
    
    .item-description {
        flex-grow: 1;
        margin-top: 5px;
        max-width: 100%;
        max-height: 100%;
        overflow-x: hidden;
    }

    .item-footer {
        text-align: center;
        margin-top: 20px;
        margin-bottom: 20px;
    }

    .empty-container {
        display: flex;
        flex-direction: column;
        align-content: center;
        align-items: center;
        padding: 0 20px 20px 20px;
    }

    .empty-text {
        text-align: center;
        margin-top: 100px;
        margin-bottom: 100px;
        font-size: 1.25rem;
    }

    .close {
        position: absolute;
        top: 0;
        right: 0;
        padding: 4px 4px 0 0;
        border: none;
        text-align: center;
        width: 18px;
        height: 14px;
        font: 16px/14px Tahoma, Verdana, sans-serif;
        color: #c3c3c3;
        text-decoration: none;
        font-weight: bold;
        background: transparent;
        cursor: pointer;
    }

    .close:hover {
        color: #999;
    }

    .button {
        border: none;
        display: inline-block;
        vertical-align: middle;
        overflow: hidden;
        text-decoration: none;
        text-align: center;
        cursor: pointer;
        white-space: nowrap;
        background-color: #04AA6D;
        color: white;
        font-family: 'Source Sans Pro', sans-serif;
        font-size: 18px;
        padding: 6px 25px;
        margin-top: 4px;
        border-radius: 5px;
        word-spacing: 10px;
    }
</style>