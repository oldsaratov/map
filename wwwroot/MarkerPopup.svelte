<script>

    export let feature;

</script>

<div class="popup">
    {#each feature.properties.items as item, i}
        <div class="photo-container">
            <div class="img-container"><a target="_blank" href="{item.url}"><img src="{item.photoUrl}" alt="{item.title}"></a></div>
            <div class="text-container">
                <p>{item.title}</p>
                {#if !item.periodFrom && !item.periodTo}
                    <span class="year-container">Дата съемки неизвестна</span>
                {:else}
                    {#if item.periodFrom === item.periodTo}
                        <span class="year-container">{item.periodFrom} год</span>
                    {:else}
                        {#if item.periodFrom && !item.periodTo}
                            <span class="year-container">{item.periodFrom} – ? годы</span>
                        {:else}
                            {#if !item.periodFrom && item.periodTo}
                                <span class="year-container">? – {item.periodTo} годы</span>
                            {:else}
                                <span class="year-container">{item.periodFrom} – {item.periodTo} годы</span>
                            {/if}
                        {/if}
                    {/if}
                {/if}
            </div>
        </div>
    {/each}
    {#if feature.properties.count > 1}
        <div class="popup-footer">
            <p>{feature.properties.count} фото</p>
        </div>
    {/if}
</div>

<style>
    .popup {
        width: 250px;
    }

    .photo-container {
        background-color: white;
        display: flex;
        flex-direction: row;
        align-items: center;
        width: 100%;
    }

    .img-container {
        height: 60px;
        margin-right: 5px;
    }

    .text-container {
        display: flex;
        flex-direction: column;
    }

    .year-container {
        font-size: .875rem;
        line-height: 1.25rem;
        font-weight: 400;
        letter-spacing: .0178571429em;
        text-decoration: inherit;
        text-transform: inherit;
        opacity: .6;
    }

    .popup-footer {
        text-align: center;
    }
</style>