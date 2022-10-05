<script setup lang="ts">
import { onMounted, Ref, ref } from 'vue'

type Jobs = {
    id: string,
    htmlFileName: string,
    status: number
}[];

const loading: Ref<boolean> = ref(false)
const jobs: Ref<Jobs | null> = ref(null)

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

</script>

<template>

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
