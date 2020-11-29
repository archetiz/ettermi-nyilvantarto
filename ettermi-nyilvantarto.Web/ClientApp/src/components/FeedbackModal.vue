<template>
  <div class="modal fade" v-bind:id="id" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="feedback-modal-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="feedback-modal-label">Visszajelzés küldése</h5>
          <button type="button" class="close" v-on:click="onHide" aria-label="Close">
          <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <div v-if="!isRated">
            <div class="form-row">
              <div class="form-group col-12">
                <h5>Köszönjük, hogy nálunk fogyasztott!</h5>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <span class="font-weight-normal">Kérjük értékelje éttermünket egy pontszámmal</span><br>
                <span class="feedback-label-comment">(1: nem ajánlanám senkinek, 5: szívesen ajánlom másoknak is)</span>
                <div class="row mt-3 mb-2">
                  <div class="col-2"></div>
                  <div class="col-8">
                    <div class="row">
                      <div class="col-1"></div>
                      <div class="col-2" @click="onRating(1)"><ion-icon :name="'star' + ((feedback.rating < 1) ? '-outline': '')"></ion-icon></div>
                      <div class="col-2" @click="onRating(2)"><ion-icon :name="'star' + ((feedback.rating < 2) ? '-outline': '')"></ion-icon></div>
                      <div class="col-2" @click="onRating(3)"><ion-icon :name="'star' + ((feedback.rating < 3) ? '-outline': '')"></ion-icon></div>
                      <div class="col-2" @click="onRating(4)"><ion-icon :name="'star' + ((feedback.rating < 4) ? '-outline': '')"></ion-icon></div>
                      <div class="col-2" @click="onRating(5)"><ion-icon :name="'star' + ((feedback.rating < 5) ? '-outline': '')"></ion-icon></div>
                      <div class="col-1"></div>
                    </div>
                  </div>
                  <div class="col-2"></div>
                </div>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group col-12">
                <span class="font-weight-normal">Kifejtheti véleményét szövegesen is</span><br>
                <span class="feedback-label-comment">(nem kötelező)</span>
                <textarea type="text" class="form-control" v-model="feedback.comment" rows="5"></textarea>
              </div>
            </div>
          </div>
          <div v-if="isRated" class="form-row">
            <div class="form-group col-12">
              <h5>Köszönjük visszajelzését!</h5>
              <p>Visszajelzését rögzítettük. Kérjük adja vissza az eszközt a pincérnek!</p>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button v-if="!isRated" type="button" class="btn btn-primary" v-on:click="onSubmit">Értékelés elküldése</button>
          <button v-else type="button" class="btn btn-primary" v-on:click="onHide">OK</button>
        </div>
      </div>
    </div>
  </div>
</template>


<script>

  export default {
    name: 'feedback-modal',

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
            orderSessionId: 0
          }
        }
      }
    },

    data() {
      return {
        isRated: false,
        feedback: {
          rating: 0,
          comment: ''
        }
      }
    },

    methods: {
      onRating: function (rating) {
        this.feedback.rating = rating;
      },

      onSubmit: function () {
        if (this.feedback.rating == 0 && this.feedback.comment.length == 0) {
          this.isRated = true;
          return;
        }

        fetch(global.App.baseURL +  'api/feedback', {
            method: 'post',
            headers: {
              'Accept': 'application/json',
              'content-type': 'application/json'
            },
            credentials: 'same-origin',
            body: JSON.stringify({
              "orderSessionId": this.options.orderSessionId,
              "rating": this.feedback.rating,
              "comment": this.feedback.comment
            })
          })
          .then(res => global.handleNetworkError(res, this))
          .then(res => {
            this.isRated = true;
          })
          .catch(err => console.log(err));
      },

      onHide: function () {
        if (this.isRated) {
          this.$emit('success-callback');
        } else {
          this.$emit('dismiss-callback');
        }
      }
    },

    watch: {
      'options.isHidden': function (val) {
        global.jQuery('#' + this.id).modal((val) ? 'hide' : 'show');
      }
    }
  }
</script>

<style>
  .modal-dialog {
    max-width: 550px;
  }
  .feedback-label-comment {
    font-size: 10pt;
  }
</style>