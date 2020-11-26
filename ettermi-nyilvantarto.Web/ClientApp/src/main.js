
window.App.user = {
  "id": 1,
  "name": "",
  "accountType":"owner", /* owner, waiter, chef */
  "isAuthenticated":true
};
window.App.name = "Éttermi Nyilvántartó";
window.App.timeFormat = 'YYYY-MM-DD HH:mm:ss';

var $ = global.jQuery = require('jquery');

import Vue from 'vue'
Vue.config.ignoredElements = [/^ion-/]
Vue.config.productionTip = false;

// VueRouter
import VueRouter from 'vue-router'
Vue.use(VueRouter)

// bootstrap
require('bootstrap');

require('./bootstrap-notify.js');
global.jQuery.notifyDefaults({
  placement: {
    from: "bottom"
  },
  animate:{
    enter: 'animated fadeInRight',
    exit: 'animated fadeOutRight'
  },
  newest_on_top: true,
  delay: 3500
});

// Global network error handler function
window.handleNetworkError = function (response) {
  // reload page if not authorized
  if (response.status == 401) {
    alert('Az oldal időközben kiléptetett a fiókodból. Kérlek jelentkezzen be újra!');
    
    this.$router.push({ path: `/login` });
  }

  if (500 <= response.status && response.status < 600) {
    // Something bad happened
    // create notification
    global.jQuery.notify({
      message: 'A kért adatok betöltése közben hiba lépett fel. Kérlek ellenőrizd, hogy nem szakadt-e meg az internetkapcsolat!'
    }, {
      type: 'danger',
      delay: 10000
    });

    return false;
  }

  return response;
};



/**
*  Helper functions
*/
global.formatMoney = function (number, decPlaces, decSep, thouSep) {
  decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
  decSep = typeof decSep === "undefined" ? "." : decSep;
  thouSep = typeof thouSep === "undefined" ? "," : thouSep;
  var sign = number < 0 ? "-" : "";
  var i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(decPlaces)));
  var j = (j = i.length) > 3 ? j % 3 : 0;

  return sign +
    (j ? i.substr(0, j) + thouSep : "") +
    i.substr(j).replace(/(\decSep{3})(?=\decSep)/g, "$1" + thouSep) +
    (decPlaces ? decSep + Math.abs(number - i).toFixed(decPlaces).slice(2) : "");
}


/**
*  APP
*/
import router from './router.js'
import App from './App.vue'

new Vue({
  el: '#app',
  router, 
  render: h => h(App)
})