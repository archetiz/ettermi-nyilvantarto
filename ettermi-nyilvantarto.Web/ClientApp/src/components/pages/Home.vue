<template><p>asd</p></template>

<script>
  export default {
    name: 'home',
    
    data() {
      return {}
    },

    mounted: function () {
      if (window.App.user.isAuthenticated == false) {
        this.$router.push({ path: `/login` });
      } else {
        if (window.App.user.accountType == 'owner') {
          this.$router.push({ path: `/feedbacks` });
        } else if (window.App.user.accountType == 'waiter') {
          this.$router.push({ path: `/new-order-session` });
        } else if (window.App.user.accountType == 'chef') {
          this.$router.push({ path: `/orders` });
        }
      }
    }
  }
</script>