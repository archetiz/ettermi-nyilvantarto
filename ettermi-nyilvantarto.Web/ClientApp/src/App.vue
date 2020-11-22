<template>
  <div id="app">
    <navbar></navbar>
    <router-view></router-view>
  </div>
</template>

<script>
import Navbar from './components/Navbar.vue'

export default {
  name: 'app',
  components: {
    'navbar': Navbar
  },
  data () {
    return {}
  },
  mounted() {
    /*let externalScript = document.createElement('script')
    externalScript.setAttribute('src', "https://unpkg.com/ionicons@4.6.4-1/dist/ionicons/ionicons.esm.js")
    document.head.appendChild(externalScript)*/
  }
}
</script>

<style lang="scss">
@import 'src/app.scss';
</style>
