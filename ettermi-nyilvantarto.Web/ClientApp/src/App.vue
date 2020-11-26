<template>
  <div id="app">
    <navbar></navbar>
    <router-view v-if="authenticated"></router-view>
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
    return {
      authenticated: false
    }
  },

  mounted: function () {
    let vm = this;
    global.Authenticate(this, function () {
      vm.authenticated = true;

      var currPath = vm.$route.path;
      if (currPath == '/') {
        if (global.App.user.accountType == 'Owner') {
          vm.$router.push({ path: `/feedbacks` });
        } else if (global.App.user.accountType == 'Waiter') {
          vm.$router.push({ path: `/new-order-session` });
        } else if (global.App.user.accountType == 'Chef') {
          vm.$router.push({ path: `/orders` });
        }
      }
    });
  }
}
</script>

<style lang="scss">
@import 'src/app.scss';
</style>
