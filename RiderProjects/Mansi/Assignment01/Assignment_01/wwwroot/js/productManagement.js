

$(document).ready(function () {
    // Function to perform the search
    $('#SearchForm').submit(function (event) {
        event.preventDefault();  // Prevent form submission
        searchProducts();
    });

    function searchProducts() {
        var searchTerm = $('#Name').val();  // Get search term from input
        var category = $('#categorySelect').val(); // Get selected category from dropdown
        var minPrice = $('#minPriceSelect').val();
        var maxPrice = $('#maxPriceSelect').val();
        var lowStock = $('#lowStockSelect').val();

        // Show the loader before the AJAX request
        $('#loader').show();

        $.ajax({
            url: '/api/ProductSearch/SearchFilter/',  // The URL for the search action
            method: 'GET',
            data: {
                // Send search term and category to the backend
                query: searchTerm,
                categoryId: category,
                minPrice: minPrice,
                maxPrice: maxPrice,
                lowStock: lowStock,
            },
            success: function (data) {
                var productHtml = '';

                if (data.length === 0) {
                    productHtml = '<div class="alert alert-info">No Products Found</div>';
                }

                // Loop through the returned products and generate HTML
                data.forEach(function (product) {
                    productHtml += '<div class="product">';
                    productHtml += '<h4>' + product.productName + '</h4>';
                    productHtml += '<p>Price: $' + product.price.toFixed(2) + '</p>';
                    productHtml += '<p>Quantity: ' + product.quantity + '</p>';
                    productHtml += '<p>Category: ' + product.category.categoryName + '</p>';
                    productHtml += '</div>';
                });

                // Update the product results on the page dynamically
                $('#productResults').html(productHtml);
            },
            error: function (xhr, status, error) {
                alert('Error: ' + xhr.responseText);
            },
            complete: function () {
                // Ensure the loader stays for at least 1 second (1000 ms)
                setTimeout(function () {
                    $('#loader').hide();
                }, 350);  // Loader stays for 5 seconds after the request completes
            }
        });
    }
});


