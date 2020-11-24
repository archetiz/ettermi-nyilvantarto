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
  }
}
</script>

<style lang="scss">
@import 'src/app.scss';
</style>
