<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="order-item-details-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="order-item-details-modal-label">{{ addNew ? "Új étel/ital hozzáadása" : "Étel/ital rendelés módosítása" }}</h5>
          <button type="button" class="close" v-on:click="onDismiss" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <div class="form-row">
            <div class="form-group col-12">
              <label for="menu-item-id" class="col-form-label">Étel/ital kiválasztása:</label>
              <select id="menu-item-id" class="form-control custom-select" :class="{'is-invalid': error_invalid_menu_item}" v-model="orderItem.menuItemId">
                <option value="null" disabled>Kérlek válassz</option>
                <optgroup v-for="m in menuItems" :label="m.categoryName">
                  <option v-for="item in m.items" :key="item.id" :value="item.id">{{ item.name }}</option>
                </optgroup>
              </select>
              <small v-if="error_invalid_menu_item" class="text-danger">
              Étel/ital kiválasztása kötelező!
              </small>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group col-12">
              <label for="order-item-quantity" class="col-form-label">Mennyiség:</label>
              <input type="number" :class="['form-control', {'is-invalid': error_quantity_out_of_bounds}]" id="order-item-quantity" v-model="orderItem.quantity" required min="1">
              <small v-if="error_quantity_out_of_bounds" class="text-danger">
              A mennyiség csak pozitív szám lehet!
              </small>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group col-12">
              <label for="order-item-comment" class="col-form-label">Komment:</label>
              <input type="text" class="form-control" id="order-item-comment" v-model="orderItem.comment">
            </div>
          </div>
          <div v-if="error_api" class="form-row">
            <div class="form-group col-12">
              <small class="text-danger">
              Nem sikerült rögzíteni az elem adatait a rendszerben. A hiba oka: {{ options.apiError }}
              </small>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button v-if="!addNew" type="button" class="btn btn-danger mr-auto" v-on:click="onDelete">Töröl</button>
          <button type="button" class="btn btn-outline-secondary" v-on:click="onDismiss">Mégsem</button>
          <button type="button" class="btn btn-primary" v-on:click="onSubmit">OK</button>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
  export default {
    name: 'order-item-details-modal',

    props: {
      id: {
        type: String,
        required: true
      },
      options: {
        type: Object,
        default: function () {
          return {
            isHidden: true,
            orderItem: {},
            apiError: ''
          }
        }
      }
    },

    data() {
      return {
        orderItem: {},
        menuItems: [],
        errors: [],
      }
    },

    computed: {
      addNew: function () {
        return (this.orderItem.orderItemId === undefined);
      },
      error_api: function () {
        return this.options.apiError.length > 0;
      },
      error_invalid_menu_item: function () {
        return this.errors.indexOf('invalid_menu_item') > -1;
      },
      error_quantity_out_of_bounds: function () {
        return this.errors.indexOf('quantity_out_of_bounds') > -1;
      }
    },

    mounted: function () {
      this.fetchMenuCategories();
    },

    methods: {
      fetchMenuCategories: function () {
        fetch(global.App.baseURL + `api/menu/categories`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            for (var i = 0; i < res.length; i++) {
              this.menuItems.push({ 
                categoryId: res[i].id, 
                categoryName: res[i].name,
                items: [] 
              });
            }
          })
          .catch(err => global.console.log(err));
      },

      fetchMenuItems: function () {
        if (this.menuItems.length == 0) {
          return;
        }

        for (var i = 0; i < this.menuItems.length; i++) {
          this.menuItems[i].items = [];
        }

        fetch(global.App.baseURL + `api/menu`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => res.json())
          .then(res => {
            for (var i = 0; i < res.length; i++) {
              for (var j = 0; j < this.menuItems.length; j++) {
                if (res[i].categoryId == this.menuItems[j].categoryId) {
                  this.menuItems[j].items.push(res[i]);

                  break;
                }
              }
            }
          })
          .catch(err => global.console.log(err));
      },

      onSubmit: function () {
        this.options.apiError = '';
        this.errors = [];

        this.orderItem.quantity = this.orderItem.quantity * 1;

        if (this.orderItem.menuItemId == null) {
          this.errors.push('invalid_menu_item');
        } else {
          this.orderItem.menuItemId = this.orderItem.menuItemId * 1;
        }
        if (this.orderItem.quantity <= 0 || this.orderItem.quantity == NaN) {
          this.errors.push('quantity_out_of_bounds');
        }

        if (this.errors.length > 0) {
          return;
        }

        this.$emit('confirm-callback', Object.assign({}, this.orderItem));
      },
      onDismiss: function () {
        this.orderItem = Object.assign({}, this.options.orderItem);
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('dismiss-callback');
      },
      onDelete: function () {
        this.options.apiError = '';
        this.errors = [];
        
        this.$emit('delete-callback', this.orderItem.orderItemId);
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      },
      'options.orderItem': function (val) {
        this.orderItem = Object.assign({
          "orderId": 0,
          "menuItemId": null,
          "quantity": 0,
          "comment": ''
        }, val);
      },
      'menuItems': function () {
        this.fetchMenuItems();
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
</style>