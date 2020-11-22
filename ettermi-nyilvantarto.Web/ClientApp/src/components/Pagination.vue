<template>
  <ul class="pagination pagination-sm">
    <li class="page-item" :class="{disabled: !data.prev_page_url}"><a class="page-link" href="javascript:void(0);" @click="callback(data.prev_page_url)">&laquo; Előző</a></li>
    <li class="page-item disabled"><a class="page-link" href="#">{{ data.current_page }} / {{ data.last_page }}</a></li>
    <li class="page-item" :class="{disabled: !data.next_page_url}"><a class="page-link" href="javascript:void(0);" @click="callback(data.next_page_url)">Következő &raquo;</a></li>
  </ul>
</template>


<script>
  export default {
    name: 'pagination',
    props: {
      data: {
        type: Object,
        default: function () {
          return {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        }
      }
    },
    methods: {
      callback: function (url) {
        this.$emit('callback', url);
      }
    }
  }
</script>
