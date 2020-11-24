<template>
  <div>
    <div class="container">
      <div class="row">
        <div class="col-12 content-box">
          <h3>Visszajelzések</h3>
        </div>
      </div>
      <div class="row">
        <div class="col-12 content-box">
          <table id="feedbacks-table" class="table table-hover table-clickable">
            <thead>
              <tr>
                <th scope="col" width="30%">Dátum</th>
                <th scope="col" width="30%">Értékelés</th>
                <th scope="col" width="30%">Rendelés azonosítója</th>
                <th width="10%"></th>
              </tr>
            </thead>
            <tbody>
              <template v-for="feedback in feedbacks">
                <tr :class="['feedbacks-table-row', 'feedback-' + feedback.id]" @click="toggleFeedback(feedback.id)">
                  <td class="font-weight-bold">{{ moment(feedback.date).format('YYYY-MM-DD HH:mm:ss') }}</td>
                  <td class="font-weight-normal"><span :class="['badge', {'badge-danger': feedback.rating <= 2}, {'badge-light': feedback.rating == 3}, {'badge-success': feedback.rating >= 4}]">{{ feedback.rating }}</span></td>
                  <td><span class="btn btn-link font-weight-bold" @click="openOrderSession(feedback.orderSessionId)">#{{ feedback.orderSessionId }}</span></td>
                  <td class="text-right">
                    <ion-icon name="caret-down-outline"></ion-icon>
                    <ion-icon name="caret-up-outline" class="d-none"></ion-icon>
                  </td>
                </tr>
                <tr :class="['feedbacks-table-details-row', 'd-none', 'feedback-' + feedback.id + '-details']">
                  <td class="font-italic">Szöveges értékelés:</td>
                  <td colspan="3" class="font-weight-light">{{ feedback.comment }}</td>
                </tr>
              </template>
              <tr v-if="feedbacks.length==0">
                <td colspan="4">
                  <span class="font-weight-normal">Nincs visszajelzés a rendszerben.</span>
                </td>
              </tr>
            </tbody>
            <caption>
              <pagination :data="pagination" @callback="paginationCallback"></pagination>
            </caption>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import PaginationComponent from './../Pagination.vue'

  var moment = require('moment');

  export default {
    name: 'feedbacks',

    components: {
      'pagination': PaginationComponent
    },

    mounted: function () {
      this.fetchFeedbacks();
    },

    data() {
      return {
        moment: moment,

        feedbacks: [],
        pagination: {
          currentPage: 1,
          data: {
            current_page: 1,
            last_page: 1,
            prev_page_url: null,
            next_page_url: null
          }
        },

        feedbackDetailsModalOptions: {
          isHidden: true,
          feedback: {},
          apiError: ''
        }
      }
    },

    methods: {
      fetchFeedbacks: function () {
        this.feedbacks = [{
          "id": 1,
          "orderSessionId": 0,
          "rating": 1,
          "comment": "string",
          "date": "2020-11-24T16:56:41.377Z"
        }];

        fetch(global.App.baseURL + `api/feedback/page/${this.pagination.currentPage}`, {
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
          })
          .then(window.handleNetworkError)
          .then(res => res.json())
          .then(res => {
            if (res.resultError === undefined) {
              //this.feedbacks = res;

              /*let pagination = {
                current_page: data.current_page,
                last_page: data.last_page,
                prev_page_url: data.prev_page_url,
                next_page_url: data.next_page_url
              };

              this.pagination = pagination;*/

              return;
            }

            // create notification
            global.jQuery.notify({
              message: 'Nem sikerült betölteni a visszajelzéseket.'
            }, {
              type: 'danger',
            });
          })
          .catch(err => global.console.log(err));
      },

      paginationCallback: function (url) {

      },

      toggleFeedback: function (id) {
        var el = document.getElementsByClassName(`feedback-${id}-details`)[0];
        if (el.classList.contains('d-none')) {
          document.getElementsByClassName(`feedback-${id}`)[0].getElementsByTagName('ion-icon')[0].classList.add('d-none');
          document.getElementsByClassName(`feedback-${id}`)[0].getElementsByTagName('ion-icon')[1].classList.remove('d-none');
          el.classList.remove('d-none');
        } else {
          document.getElementsByClassName(`feedback-${id}`)[0].getElementsByTagName('ion-icon')[0].classList.remove('d-none');
          document.getElementsByClassName(`feedback-${id}`)[0].getElementsByTagName('ion-icon')[1].classList.add('d-none');
          el.classList.add('d-none');
        }
      },

      openOrderSession: function (id) {
        this.$router.push({ path: `/order-session/${id}` });
      }
    }
  }
</script>

<style>
</style>