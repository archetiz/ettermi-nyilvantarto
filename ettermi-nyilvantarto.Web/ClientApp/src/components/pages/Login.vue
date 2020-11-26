<template>
  <div class="text-center full-width">
    <form v-on:submit.prevent="" class="form-signin">

      <h1 class="h3 mb-3 font-weight-normal">Bejelentkezés</h1>

      <label for="username" class="sr-only">Felhasználónév</label>
      <input v-model="userName" type="text" id="username" name="username" :class="['form-control', {'has-error': isLoginFailed}]" placeholder="Felhasználónév" required autofocus>
      
      <label for="password" :class="['sr-only', {'has-error': isLoginFailed}]">Jelszó</label>
      <input v-model="password" type="password" id="password" name="password" class="form-control" placeholder="Jelszó" required>
      
      <div class="mb-3 text-left">
        <span v-if="isLoginFailed" class="help-block text-left">
          <strong>Hibás bejelentkezési adatok. Kérlek próbáld újra!</strong>
        </span>
      </div>
      
      <button @click="login" class="btn btn-lg btn-primary btn-block" type="submit">Belépek</button>
    </form>
  </div>
</template>

<script>
  export default {
    name: 'login',

    data() {
      return {
        userName: '',
        password: '',
        isLoginFailed: false
      }
    },
    
    methods: {
      login: function() {
        this.isLoginFailed = false;

        fetch(global.App.baseURL + 'api/user/login', {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: `{"userName":"${this.userName}","password":"${this.password}"}`
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            if (res.isSuccess) {
              let vm = this;
              global.Authenticate(this, function () {
                if (global.App.user.accountType == 'Owner') {
                  vm.$router.push({ path: `/feedbacks` });
                } else if (global.App.user.accountType == 'Waiter') {
                  vm.$router.push({ path: `/new-order-session` });
                } else if (global.App.user.accountType == 'Chef') {
                  vm.$router.push({ path: `/orders` });
                }
              });
              
              return;
            }

            this.isLoginFailed = true;
          })
          .catch(err => global.console.log(err));
      }
    }
  }
</script>

<style>
  html,
  body {
    height: 100%;
  }

  #app {
    width: 100%;
  }

  .text-center.full-width {
    width: 100%;
  }

  .form-signin {
    width: 100%;
    max-width: 330px;
    padding: 15px;
    margin: auto;
    padding-top: 40px;
    padding-bottom: 40px;
  }
  .form-signin .checkbox {
    font-weight: 400;
  }
  .form-signin .form-control {
    position: relative;
    box-sizing: border-box;
    height: auto;
    padding: 10px;
    font-size: 16px;
  }
  .form-signin .form-control:focus {
    z-index: 2;
  }
  .form-signin input[type="email"] {
    margin-bottom: -1px;
    border-bottom-right-radius: 0;
    border-bottom-left-radius: 0;
  }
  .form-signin input[type="password"] {
    margin-bottom: 10px;
    border-top-left-radius: 0;
    border-top-right-radius: 0;
  }
</style>