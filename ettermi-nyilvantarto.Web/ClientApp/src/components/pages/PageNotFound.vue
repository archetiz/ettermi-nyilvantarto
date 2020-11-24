<template>
  <div class="container">
      <div class="row">
          <div class="col-md-12">
              <div class="error-template">
                  <h1>
                      Oops!</h1>
                  <h2>
                      404 Az oldal nem található</h2>
                  <div class="error-details">
                      Elnézést, hiba történt. A kért oldal nem létezik.
                  </div>
              </div>
          </div>
      </div>
  </div>
</template>


<script>
  export default {
    name: 'page-not-found',
    
    data() {
      return {}
    }
  }
</script>

<style>
  .error-template {padding: 40px 15px;text-align: center;}
  .error-actions {margin-top:15px;margin-bottom:15px;}
  .error-actions .btn { margin-right:10px; }
</style>