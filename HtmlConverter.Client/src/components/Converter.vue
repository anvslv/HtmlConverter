<script setup lang="ts">
import { onMounted, Ref, ref, computed, ComputedRef } from 'vue'
import ConverterHub from '../hubs/converterHub'
import List from './List.vue'
import ListItem from './ListItem.vue'

type Jobs = {
    id: number,
    htmlFileName: string,
    status: string
}[];

const loading: Ref<boolean> = ref(false)
const jobs: Ref<Jobs | null> = ref(null)
const file: Ref<File | null> = ref(null);
const form: Ref<HTMLFormElement | null> = ref(null);

const sortedJobs: ComputedRef<Jobs | null> = computed(() => {
    if (jobs.value == null) {
        return []
    }

    return jobs.value.sort((x: { id: number }, y: { id: number }) => (x.id > y.id ? -1 : 1));
})

onMounted(() => {
    fetchJobs();

    ConverterHub.client.on("NewConversionJob", function (newJob) {
        console.log("New job", newJob)
        jobs.value?.push(newJob)
    })

    ConverterHub.client.on("ConversionStatusChanged", function (jobId, jobStatus) {
        console.log("Job status changed", jobId, jobStatus)

        if (jobs.value) {
            let updated = jobs.value.find((item: { id: number }) => item.id == jobId);

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

    fetch(import.meta.env.VITE_SERVER_URL + '/api/converterjobs/jobs')
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


    fetch(import.meta.env.VITE_SERVER_URL + '/api/converterjobs/createjob', {
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

    <div class="p-10 m-auto">

        <h1 class="text-3xl">Input HTML file</h1>

        <form ref="form" class="mt-10">
            <input ref="file" @change="onFileChanged($event)" type="file" class="
                    form-input 
                    px-1 
                    py-1  
                    mt-1
                    block
                    w-full
                    rounded-md
                    bg-gray-100
                    border-transparent
                    focus:border-gray-500 focus:bg-white focus:ring-0
                  " />
        </form>

        <div class="mt-10">
            <div v-if="loading">
                <h1 class="text-3xl">
                    Loading...
                </h1>
            </div>

            <div v-if="sortedJobs">

                <h1 v-if="!loading" class="text-3xl">Conversion Jobs</h1>

                <List>
                    <ListItem v-for="job in sortedJobs" :key="job.id" :jobId="job.id" :status="job.status"
                        :html-file-name="job.htmlFileName" />
                </List>
            </div>
        </div>

    </div>
</template>

<style scoped>

</style>
