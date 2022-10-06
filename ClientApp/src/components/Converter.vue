<script setup lang="ts">
import { onMounted, Ref, ref } from 'vue'
import BeatLoader from 'vue-spinner/src/BeatLoader.vue'
import ConverterHub from '../hubs/converterHub'

type Jobs = {
    id: string,
    htmlFileName: string,
    status: string
}[];

const loading: Ref<boolean> = ref(false)
const jobs: Ref<Jobs | null> = ref(null)
const file: Ref<File | null> = ref(null);
const form: Ref<HTMLFormElement | null> = ref(null);

onMounted(() => {
    fetchJobs();

    ConverterHub.client.on("NewConversionJob", function (newJob) {
        console.log("New job", newJob)
        jobs.value?.push(newJob)
    })

    ConverterHub.client.on("ConversionStatusChanged", function (jobId, jobStatus) {
        console.log("Job status changed", jobId, jobStatus)

        if (jobs.value) {
            let updated = jobs.value.find(item => item.id == jobId);

            if (updated) {
                console.log("Job status updated", jobId, jobStatus)
                updated.status = jobStatus;
            }
        }
    })

    ConverterHub.start();
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
            success => {
                console.log(success);
                file.value = null;
                if (form.value instanceof HTMLFormElement) {
                    form.value.reset();
                }
            }
        ).catch(
            error => {
                console.log(error);
                file.value = null;
                if (form.value instanceof HTMLFormElement) {
                    form.value.reset();
                }

            }
        );
}


</script>

<template>
    <form ref="form">
        <input ref="file" @change="onFileChanged($event)" type="file">
    </form>

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
                        <beat-loader v-if="job.status != 'Done'" :loading="true" :color="'#ccc'" :size="'8px'">
                        </beat-loader>

                        <a v-if="job.status == 'Done'" target="_blank"
                            :href="`/api/converterjobs/pdf/${job.id}`">PDF</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

</template>

<style scoped>

</style>
