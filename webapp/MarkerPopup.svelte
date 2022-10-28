<script>
    import Period from "./Period.svelte";
    
    export let item;

</script>

<div class="popup">
    <div class="img-container">
        <a target="_blank" href="{item.url}">
            <img src="{item.photoUrl}" alt="{item.title}">
        </a>
    </div>
    <div>
        <h3>{item.title}</h3>
    </div>
    <div>
        <Period bind:periodFrom={item.periodFrom} bind:periodTo={item.periodTo} />
    </div>
</div>

<style>
    .popup {
        width: 210px;
        display: flex;
        flex-direction: column;
    }
    
    .img-container img {
        -o-object-fit: cover;
        object-fit: cover;
        border-radius: 4px;
        height: 160px;
        width: 100%;
        background: #e6e3e3;
    }
</style>