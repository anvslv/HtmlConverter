import { defineConfig, loadEnv } from "vite";
import vue from '@vitejs/plugin-vue' 

import fs from "fs";
import path from "path";
 
export default ({ mode }) => {
    process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };
     
    const baseFolder = "./cert";

    const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
    const certificateName = certificateArg ? certificateArg.groups.value : "htmlconverter.client";

    if (!certificateName) {
        console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
        process.exit(-1);
    }

    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    return defineConfig({
        plugins: [vue()],
        server: {
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            },
            port: process.env.VITE_CLIENT_PORT,
            host: true
        },
    });
}


  