<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a :href="App.baseURL"><img :src="App.baseURL + 'img/app-icon/android-chrome-192x192.png'" class="logo" height="30px"></a>
    <a class="navbar-brand" :href="App.baseUrl">{{ App.name }}</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="menu-bar collapse navbar-collapse" id="navbarSupportedContent">
      <ul class="navbar-nav mr-auto">
        <router-link class="nav-item" tag='li' to='/'>
          <a class="nav-link">Menü elem</a>
        </router-link>
      </ul>
      <!-- Authentication Links -->
      <ul v-if="!App.user.isAuthenticated" class="navbar-nav">
        <li class="nav-item"><a href="javascript:void();" @click="loginBtn" class="nav-link text-danger">Bejelentkezés</a></li>
      </ul>
      <!-- Authentication Links -->
      <ul v-else class="navbar-nav">
        <li class="nav-item dropdown">
          <a href="#" class="nav-link dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">
            {{ App.user.name }} <span class="caret"></span>
          </a>
          <div class="dropdown-menu" aria-labelledby="navbarDropdown" role="menu">
            <a class="dropdown-item" href="#"
              onclick="event.preventDefault();
              document.getElementById('logout-form').submit();">
              Kijelentkezés
            </a>
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
      loginBtn: function () {
        let urlHash = encodeURIComponent('#' + window.location.href.split('#')[1]);
        window.location.replace(window.route('login') + '?redirect=' + urlHash);
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