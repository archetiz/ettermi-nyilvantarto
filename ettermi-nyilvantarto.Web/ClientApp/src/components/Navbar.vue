<template>
  <nav v-if="App.user.isAuthenticated" class="navbar navbar-expand-lg navbar-light bg-light">
    <a :href="App.baseURL"><img :src="App.baseURL + 'img/app-icon/android-chrome-192x192.png'" class="logo" height="30px"></a>
    <a class="navbar-brand" :href="App.baseUrl">{{ App.name }}</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="menu-bar collapse navbar-collapse" id="navbarSupportedContent">
      <ul v-if="App.user.isAuthenticated" class="navbar-nav mr-auto">
        <router-link v-if="isOwner" class="nav-item" tag="li" to="/vouchers">
          <a class="nav-link">Kuponok</a>
        </router-link>
        <router-link v-if="isOwner" class="nav-item" tag="li" to="/feedbacks">
          <a class="nav-link">Visszajelzések</a>
        </router-link>
        <router-link v-if="isOwner || isChef" class="nav-item" tag="li" to="/menu">
          <a class="nav-link">Menü</a>
        </router-link>
        <router-link v-if="isOwner || isWaiter" class="nav-item" tag="li" to="/reservations">
          <a class="nav-link">Foglalások</a>
        </router-link>
        <router-link v-if="isChef" class="nav-item" tag="li" to="/orders">
          <a class="nav-link">Rendelések</a>
        </router-link>
        <li v-if="isOwner || isWaiter" class="nav-item dropdown">
          <a class="nav-link dropdown-toggle" href="javascript:void(0);" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            Rendelések
          </a>
          <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
            <router-link class="dropdown-item" href="javascript:void(0);" tag="a" to="/new-order-session">Új felvétele</router-link>
            <router-link class="dropdown-item" href="javascript:void(0);" tag="a" to="/order-sessions">Meglévők listázása</router-link>
            <router-link class="dropdown-item" href="javascript:void(0);" tag="a" to="/pay-order-session">Fizetés</router-link>
          </div>
        </li>
        <router-link v-if="isOwner || isWaiter" class="nav-item" tag="li" to="/customers">
          <a class="nav-link">Megrendelők</a>
        </router-link>
        <router-link v-if="isOwner || isWaiter" class="nav-item" tag="li" to="/royalty-card-balance">
          <a class="nav-link">Hűségkártya egyenleg</a>
        </router-link>
      </ul>
      <!-- Authentication Links -->
      <ul class="navbar-nav">
        <li class="nav-item dropdown">
          <a href="#" class="nav-link dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
            {{ App.user.name }} <span class="caret"></span>
          </a>
          <div class="dropdown-menu" aria-labelledby="navbarDropdown" role="menu">
            <a class="dropdown-item" :click="logoutBtn" href="javascript:void(0);">Kijelentkezés</a>
          </div>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script>
  export default {
    name: 'navbar',

    data() {
      return {
        App: window.App
      }
    },
    
    methods: {
      logoutBtn: function () {
        fetch(this.App.baseURL + 'api/user/logout', {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => {
            if (res.status == 200) {
              window.App.user.name = "";
              window.App.user.accountType = "";
              window.App.user.isAuthenticated = false;

              this.$router.push({ path: `/login` });
              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült kijelentkezni.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      }
    },
    computed: {
      isOwner: function () {
        return this.App.user.accountType == 'owner';
      },
      isWaiter: function () {
        return this.App.user.accountType == 'waiter';
      },
      isChef: function () {
        return this.App.user.accountType == 'chef';
      }
    }
  }
</script>

<style>
  .logo {
    margin: 10px 15px 10px 0;
  }
  .menu-bar {
    font-size: 1.1em;
  }
</style>