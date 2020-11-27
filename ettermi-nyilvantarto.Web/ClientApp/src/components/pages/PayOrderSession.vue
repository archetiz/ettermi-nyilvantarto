<template>
  <div>
    <div class="container">
      <div class="row new-order-session">
        <div class="d-none d-lg-block col-lg-2"></div>
        <div class="content-box col-12 col-lg-8">
          <div class="row">
            <div class="col-12">
              <h3 class="text-center">Fizetés</h3>
            </div>
          </div>
          <div class="row">
            <div class="content-box col-12">
              <h4 class="mb-2">Asztal kiválasztása</h4>
              <div v-for="tg in tables">
                <div class="row">
                  <div class="col-12">
                    <h5>{{ tg.categoryName }}</h5>
                  </div>
                </div>
                <div class="row">
                  <div v-for="table in tg.items" class="col-6 col-lg-3">
                    <span class="btn btn-light btn-block pt-4 pb-4 new-btn" @click="onTableSelected(table.id)" role="button">{{ table.code }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="content-box d-none d-lg-block col-lg-2"></div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'new-order-session',

    data() {
      return {
        tables: []
      }
    },

    mounted: function () {
      this.fetchTableCategories();
    },

    methods: {
      fetchTableCategories: function () {
        fetch(global.App.baseURL + `api/table/categories`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            if (res.resultError === undefined) {
              for (var i = 0; i < res.length; i++) {
                this.tables.push({ 
                  categoryId: res[i].id, 
                  categoryName: res[i].name,
                  items: [] 
                });
              }

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni az asztalkategóriákat.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      fetchTableList: function () {
        for (var i = 0; i < this.tables.length; i++) {
          this.tables[i].items = [];
        }

        fetch(global.App.baseURL + `api/table`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            res = [
              {
                "id": 1,
                "code": "A",
                "size": 2,
                "categoryId": 1,
                "categoryName": "CatA"
              },
              {
                "id": 2,
                "code": "AA",
                "size": 2,
                "categoryId": 1,
                "categoryName": "CatA"
              },
              {
                "id": 3,
                "code": "B",
                "size": 2,
                "categoryId": 2,
                "categoryName": "CatB"
              }
            ];

            if (res.resultError === undefined) {
              for (var i = 0; i < res.length; i++) {
                for (var j = 0; j < this.tables.length; j++) {
                  if (res[i].categoryId == this.tables[j].categoryId) {
                    this.tables[j].items.push(res[i]);

                    break;
                  }
                }
              }

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni az asztallistát.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      onTableSelected: function (id) {
        fetch(global.App.baseURL + `api/table/${id}/session`, {
            method: 'get',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            if (res.resultError !== undefined) {
              return;
            }

            this.$router.push({ path: `/order-session/${res}` });
          })
          .catch(err => console.log(err));
      }
    },

    watch: {
      'tables': function () {
        this.fetchTableList();
      }
    }
  }
</script>

<style>
  .new-order-session .new-btn {
    font-size: 16pt;
  }
</style>