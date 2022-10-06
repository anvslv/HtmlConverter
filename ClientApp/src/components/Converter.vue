<script setup lang="ts">
import { onMounted, Ref, ref } from 'vue'

type Jobs = {
    id: string,
    htmlFileName: string,
    status: number
}[];

const loading: Ref<boolean> = ref(false)
const jobs: Ref<Jobs | null> = ref(null)
const file: Ref<File | null> = ref(null);

onMounted(() => {
    fetchJobs();
})

function fetchJobs() {
    jobs.value = null;
    loading.value = true;

    fetch('api/converterjobs/jobs')
        .then(r => r.json())
        .then(json => {
            jobs.value = json as Jobs;
            loading.value = false;
            return;
        });
}

function onFileChanged($event: Event) {
    const target = $event.target as HTMLInputElement;
    if (target && target.files) {
        file.value = target.files[0];
    }
    else {
        file.value = null;
    }

    if (file.value == null) {
        console.log("No files selected");

        return;
    }

    console.log("Selected files", file.value)

    const payload = new FormData();
    payload.append('file', file.value, file.value.name);


    fetch('/api/converterjobs/createjob', {
        method: 'POST',
        body: payload
    })
        .then(
            response => response.json()
        ).then(
            success => console.log(success)
        ).catch(
            error => console.log(error)
        );
}


</script>

<template>

    <input ref="file" @change="onFileChanged($event)" type="file">

    <div v-if="loading" class="loading">
        Loading...
    </div>

    <div v-if="jobs" class="content">
        <table>
            <thead>
                <tr>
                    <th>File</th>
                    <th>Status</th>
                    <th>PDF</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="job in jobs" :key="job.id">
                    <td>{{ job.htmlFileName }}</td>
                    <td>{{ job.status }}</td>
                    <td>
                        <a target="_blank" href="api/converterjobs/pdf">PDF</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

</template>

<style scoped>

</style>
