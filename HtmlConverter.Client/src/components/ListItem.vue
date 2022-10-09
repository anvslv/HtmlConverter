<template>
    <article class="flex items-start space-x-6 p-6">
        <span class="bg-icon-html w-14 h-14 bg-no-repeat inline-block "></span>
        <div class="min-w-0 relative flex-auto">
            <h2 class="font-semibold text-slate-900 pr-20"> {{ htmlFileName }}</h2>
            <dl class="mt-2 flex flex-wrap text-sm leading-6 font-medium">
                <div>
                    <dt class="sr-only">Status</dt>
                    <dd class="flex items-center text-gray-400"> 
                        {{ status }}
                    </dd>
                </div>

                <div v-if="status != 'Done'">
                    <dt class="sr-only">Loading</dt>
                    <dd class="flex items-center">
                        <svg width="2" height="2" fill="currentColor" class="mx-2 text-slate-300" aria-hidden="true">
                            <circle cx="1" cy="1" r="1" />
                        </svg>
                        <beat-loader :loading="status != 'Done'" :color="'#ccc'" :size="'6px'">
                        </beat-loader>
                    </dd>
                </div>

            </dl>
        </div>
        <span v-if="status == 'Done'" class="bg-icon-pdf w-14 h-14 bg-no-repeat inline-block "></span>
        <div v-if="status == 'Done'" class="min-w-0 relative flex-auto">
            <h2 class="font-semibold text-slate-900  pr-20">
                <a target="_blank" class="underline underline-offset-2 text-blue-600"
                    :href="`${server}/api/converterjobs/pdf/${jobId}`">PDF</a>
            </h2>
        </div>

        <span v-if="status != 'Done'" class="opacity-30 bg-icon-pdf w-14 h-14 bg-no-repeat inline-block "></span>
        <div v-if="status != 'Done'" class="opacity-30 min-w-0 relative flex-auto">
            <h2 class="font-semibold text-slate-900  pr-20">
                PDF
            </h2>
        </div>

    </article>
</template>
  
<script lang="ts" setup>
import BeatLoader from 'vue-spinner/src/BeatLoader.vue'

const server = import.meta.env.VITE_SERVER_URL

interface Job {
    jobId: number,
    htmlFileName: string,
    status: string
}

defineProps<Job>()
</script>