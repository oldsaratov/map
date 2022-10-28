<script>

    export let periodFrom;
    export let periodTo;

</script>

<div>
    {#if !periodFrom && !periodTo}
        <span class="year-container">Дата съемки неизвестна</span>
    {:else}
        {#if periodFrom === periodTo}
            <span class="year-container">{periodFrom} год</span>
        {:else}
            {#if periodFrom && !periodTo}
                <span class="year-container">{periodFrom} – ? годы</span>
            {:else}
                {#if !periodFrom && periodTo}
                    <span class="year-container">? – {periodTo} годы</span>
                {:else}
                    <span class="year-container">{periodFrom} – {periodTo} годы</span>
                {/if}
            {/if}
        {/if}
    {/if}
</div>

<style>
    .year-container {
        font-size: .875rem;
        line-height: 1.25rem;
        font-weight: 400;
        letter-spacing: .0178571429em;
        text-decoration: inherit;
        text-transform: inherit;
        opacity: .6;
    }
</style>