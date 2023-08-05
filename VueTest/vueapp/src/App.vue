<template>
  <div>
    <ul>
      <li v-for="region in regions" :key="region">
        {{ region.name }}
      </li>
    </ul>
  </div>
</template>

<script lang="ts" setup>
  import { reactive, onMounted } from "vue";

  const regions = reactive([]);

  onMounted(async () => {
    const result = await fetch("http://localhost:5000/api/regions");
    const data: Array<never> = await result.json();
    regions.splice(0, regions.length, ...data);
  });
</script>

<style>
</style>
