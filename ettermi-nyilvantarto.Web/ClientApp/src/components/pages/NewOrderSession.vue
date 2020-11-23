<template>
  <div>
    <div class="container">
      <div class="row new-order-session">
        <div class="d-none d-lg-block col-lg-2"></div>
        <div class="col-12 col-lg-8 content-box">
          <div class="row">
            <div class="col-12">
              <h3 class="text-center mb-3">Új rendelés felvétele</h3>
            </div>
          </div>
          <div class="row">
            <div :class="['col-12', 'mb-2', {'d-none': (orderType == '')}]">
              <button type="button" class="btn btn-secondary d-none d-lg-block" v-on:click="onBackButton">Vissza</button>
              <button type="button" class="btn btn-secondary btn-block d-lg-none" v-on:click="onBackButton">Vissza</button>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != '')}]">
            <div class="col-12 col-lg-6 mb-3">
              <a class="btn btn-light btn-block pt-4 pb-4 new-btn" href="" @click="takeawaySelected" role="button">Elvitelre</a>
            </div>
            <div class="col-12 col-lg-6 mb-3">
              <a class="btn btn-light btn-block pt-4 pb-4 new-btn" href="" @click="onsiteSelected" role="button">Helyben fogyasztás</a>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != 'takeaway')}]">
            <div class="col-12">
              <h4 class="mb-2">Keresés a rendszerben</h4>
              <form>
                <div class="form-row">
                  <div class="form-group col-12 col-lg-10">
                    <input type="text" class="form-control" v-model="serachQuery" required>
                  </div>
                  <div class="form-group col-12 col-lg-2">
                    <button type="button" class="btn btn-primary btn-block" v-on:click="onSearchCustomer">Keresés</button>
                  </div>
                </div>
              </form>
              <table class="table table-hover table-clickable">
                <thead>
                  <tr>
                    <th scope="col">Név</th>
                    <th scope="col">Telefonszám</th>
                    <th scope="col">Cím</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="customer in searchResults" :key="customer.id" @click="onSelectCustomer(customer.id)">
                    <td class="font-weight-normal">{{ customer.name.substring(0, 30) }}{{ ((customer.name.length > 30) ? '...' : '') }}</td>
                    <td class="font-weight-normal">{{ customer.phoneNumber }}</td>
                    <td class="font-weight-normal">{{ customer.address.substring(0, 30) }}{{ ((customer.name.length > 30) ? '...' : '') }}</td>
                    <td>
                      <ion-icon name="play"></ion-icon>
                    </td>
                  </tr>
                  <tr v-if="searchResults.length==0">
                    <td colspan="4">
                      <span class="font-weight-normal">Nincs a keresésnek megfelelő megrendelő a rendszerben.</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          <div :class="['row', {'d-none': (orderType != 'takeaway')}]">
            <div class="col-12">
              <h4 class="mb-2">Új megrendelő hozzáadása</h4>
              <form>
                <div class="form-row">
                  <div class="form-group col-12">
                    <label for="customer-name" class="col-form-label">Név:</label>
                    <input type="text" class="form-control" id="customer-name" v-model="customer.name" required>
                  </div>
                  <div class="form-group col-12">
                    <label for="customer-phone" class="col-form-label">Telefonszám:</label>
                    <input type="text" :class="['form-control', {'is-invalid': error_phone_wrong_format}]" id="customer-phone" v-model="customer.phoneNumber" pattern="[+]?[0-9]*" required>
                    <small v-if="error_phone_wrong_format" class="text-danger">
                    A megadott formátum nem megfelelő!
                    </small>
                    <small class="text-info">
                    Formátum: +36201234567
                    </small>
                  </div>
                  <div class="form-group col-12">
                    <label for="customer-address" class="col-form-label">Cím:</label>
                    <input type="text" class="form-control" id="customer-address" v-model="customer.address" required>
                  </div>
                  <div v-if="error_add_new_customer_failing" class="form-group col-12">
                    <small class="text-danger">
                    Nem sikerült rögzíteni a vásárlót a rendszerben.
                    </small>
                  </div>
                  <div class="form-group col-12">
                    <button type="button" class="btn btn-primary d-none d-lg-block" v-on:click="onSubmitNewCustomer">Hozzáad</button>
                    <button type="button" class="btn btn-primary btn-block d-lg-none" v-on:click="onSubmitNewCustomer">Hozzáad</button>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
        <div class="d-none d-lg-block col-lg-2"></div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'new-order-session',
    data() {
      return {
        orderType: '', // takeaway/onsite
        customer: {
          name: '',
          phoneNumber: '',
          address: ''
        },
        errors: [],
        searchQuery: '',
        searchResults: []
      }
    },
    computed: {
      error_phone_wrong_format: function () {
        return this.errors.indexOf('phone_wrong_format') > -1;
      },
      error_add_new_customer_failing: function () {
        return this.errors.indexOf('add_new_customer_failing') > -1;
      }
    },
    methods: {
      takeawaySelected: function (e) {
        e.preventDefault();

        this.orderType = 'takeaway';
      },
      onsiteSelected: function (e) {
        e.preventDefault();

        this.orderType = 'onsite';
      },
      onSubmitNewCustomer: function () {
        this.errors = [];

        if (!/^([+]?[0-9]{9,11})$/.test(this.customer.phoneNumber)) {
           this.errors.push('phone_wrong_format');
        }

       this.errors.push('add_new_customer_failing');
       
        alert(0);
      },
      onSearchCustomer: function () {
       
        alert(0);
      },
      onBackButton: function () {
        this.orderType = '';
      }
    }
  }
</script>

<style>
  .new-order-session .new-btn {
    font-size: 16pt;
  }
</style>