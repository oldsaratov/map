import svelte from "rollup-plugin-svelte";
import resolve from "@rollup/plugin-node-resolve";
import { terser } from "rollup-plugin-terser";
import commonjs from '@rollup/plugin-commonjs';
import copy from "rollup-plugin-copy";
import fs from "fs";
import posthtml from "posthtml";
import { hash } from "posthtml-hash";
import rimraf from "rimraf";

const OUT_DIR = 'wwwroot';
const OUT_FILE = `${OUT_DIR}/index.html`;

const hashStatic = () => ({
    name: 'hash-static',
    buildStart: () => rimraf.sync(OUT_DIR),
    writeBundle: () => {
        posthtml([hash({ path: OUT_DIR })])
            .process(fs.readFileSync(OUT_FILE, 'utf-8'))
            .then((result) => fs.writeFileSync(OUT_FILE, result.html));
    },
});

const production = !process.env.ROLLUP_WATCH;

export default {
    input: `webapp/main.js`,
    output: {
        sourcemap: false,
        format: 'iife',
        name: 'app',
        file: `${OUT_DIR}/bundle.[hash].js`,
    },
    plugins: [
        copy({ targets: [
                { src: 'webapp/index.template.html', dest: OUT_DIR, rename: 'index.html' },
                { src: 'webapp/global.css', dest: OUT_DIR }
            ] }),
        svelte({
            compilerOptions: {
                dev: !production,
            },
            emitCss: false,
        }),
        commonjs(),
        resolve({
            browser: true,
            dedupe: ['svelte']
        }),
        production && terser(),
        hashStatic(),    
    ],
    watch: {
        clearScreen: false
    }
};
