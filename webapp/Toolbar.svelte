<script>
    import RangeSlider from "svelte-range-slider-pips"
    import Switch from "svelte-switch";
    import { createEventDispatcher } from 'svelte';

    const dispatch = createEventDispatcher();

    let bound = [1863, new Date().getFullYear()];
    export let sidebarActive = false;

    function onDisplayModeChanged(e) {
        dispatch('displayModeChanged', {
            active: e.detail.checked
        });
        
        sidebarActive = e.detail.checked;
    }
    
    function onRangeChanged() {
        dispatch('rangeChanged', {
            bound: bound
        });
    }

</script>

<div class="toolbar">
    <div class="toolbar-slider">
        <RangeSlider id="reverse-pips"
                     range pushy float
                     pips step={1} pipstep={10}
                     min={1860} max={new Date().getFullYear()}
                     first='label' last={false} rest='label'
                     bind:values={bound}
                     on:stop={onRangeChanged}/>
    </div>
    <div class="toolbar-toggle">
        <Switch on:change={onDisplayModeChanged} checked={sidebarActive} onColor="#888888">
            <img title="Всплывающее окно" class="toggle-icon" slot="unCheckedIcon" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABmJLR0QA/wD/AP+gvaeTAAACDklEQVR4nO2bvUoDQRRGTyQJ2AjWtjYWFmKlxF5RbAVbK3vBXsTEPp2ddr6AhVhJHsDGB/AXEUREsFDUYiYkxjG56u7c7OYeGDbM3N395mN29s5mFwzDML5SBnaBG+AjJ+UaqPm+9aTWB4LTKlWJAdc+eFYSnBEqtEZCT5pu5Y1gv4YEOzaA0xzUiel0KuRcXupEIyDXFAUxDeA9B3VibBIcJLpdAnkcBd+wEdClrRBNRRyCI3rgR4AZoC1AGzNAW4A2ZsA/9l0EroBLYCEZOf2BdC1w2RZ7EWhvtLU3y2mP9r8U6bo/+logZGLnc4a0zvPvA0oOuoAbBRfAfNIiUkC8yrXl8CBhBmgL0MbygABp5gFplEzlAZk4j+UB0sCMYXlACDNAW4A2lgcE6Jc84M//7/9A5vIAtTuR5QHSwIxheUAIM0BbgDZmgLYAbcwAbQHahAy48dtKTCEpM+e3opelqyT7zO4OqAPDvxA8ChwADwlr2ZGcvOxNaL42n1Q5QfbBwghwlvC5r3znRR9MJM0ELTP36f72WRE48rHnwFjq6iIxCTziOrbVJa7uY+6B8Qi6ojIPvOI6uB5o3/BtL8BMRF1RWcN18g1Ybqtf8nXvwKqCrqhs40x4AqaAaeDZ120q6opGATcZfuAmx1v/e09TVGzKwDGt29QRUNIQovlCdAlYwV33h7g5wDAi8wmt/Leysjc8EgAAAABJRU5ErkJggg==">
            <img title="Боковая панель" class="toggle-icon" slot="checkedIcon" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAABmJLR0QA/wD/AP+gvaeTAAABWElEQVR4nO3WMU4DMRBG4QfiBFCSmpZrEHFBzkBKWioQ9LSUCSfIAYBig6BZBpjYeNfvk6ItNpGtief3gCRJkiRJ0p4tgBWwBd46/Yw6+kHxnoDj4HsasWL4B26A0wrrPQD3Fdap5qNtaxQPhuLdVVqrijADenf43xuYutYKOLkMbK2Ar8wsMszAQOYEXgIvwAZY7mc78/PdCdx8eb8e+d1vJ3wzMKmrDFwynMI1cFFtRxPjJRJorYXNwKSuMlA4BxbnHBgwAwtzDkzyEgm01sJmYFJXGSicA4tzDgyYgYU5ByZ5iQRaa2EzMKmrDBTOgcU5BwbMwMKcA5O8RAKttbAZmNRVBgrnwOKcAwNmYGHOgUleIoHWWtgMTOoqA0V7J3ByogJud89F6Y3szC4Db3fPK+oUcXIZeBC8PwMegZMKe2nZaJ2iE/gMnAPXfLazJEmSJEmS/uodb9wAm8SUICoAAAAASUVORK5CYII=">
        </Switch>
    </div>
</div>

<style>
    .toolbar {
        height: 100px;
        display: flex;
        flex-direction: row;
        align-items: center;
        z-index: 9999;
        box-shadow: -3px 12px 10px -13px rgba(166 166 166 / 50%);
        padding-left: 10px;
        padding-right: 10px;
        position: relative;
    }

    .toolbar-slider {
        margin-top: 50px;
        margin-bottom: 50px;
        width: 100%;
    }

    .toolbar-toggle {
        width: 60px;
    }
    
    .toggle-icon {
        display: flex; 
        justify-content: center; 
        align-items: center;
        width: 22px;
        height: 22px;
        padding: 3px;
    }

    #reverse-pips .rangePips {
        bottom: auto;
        top: -1em;
    }

    #reverse-pips .pip {
        background: rgb(198, 187, 224);
        top: auto;
        bottom: 0.25em;
        width: 2px;
        transform: translateX(-1px);
        transition-duration: 0.5s;
        opacity: 0.7;
    }

    #reverse-pips .pip:nth-child(5n+1) {
        height: 0.8em;
        opacity: 0.9;
    }

    #reverse-pips .pip:nth-child(5n),
    #reverse-pips .pip:nth-child(5n+2) {
        height: 0.65em;
    }

    #reverse-pips .pip.selected {
        background: rgb(255, 0, 157);
        transition-duration: 0.05s;
        opacity: 1;
    }
</style>