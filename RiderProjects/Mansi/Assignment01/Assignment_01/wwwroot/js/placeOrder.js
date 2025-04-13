
$(document).ready(function () {
    // Handle quantity change
    $('input[name^="OrderQuantities"]').on('input', function () {
        var row = $(this).closest('tr');
        var productId = row.find('input[name^="ProductIds"]').val();
        var quantityToOrder = $(this).val();
        var availableQuantity = row.find('.available-quantity').text(); // You can add this to display the available quantity

        // If the quantity to order is greater than the available quantity, show an alert
        if (parseInt(quantityToOrder) > parseInt(availableQuantity)) {
            alert('You cannot order more than the available stock!');
            $(this).val(0); // Reset the quantity input
        }


        console.log({
            productId: productId,
            quantityToOrder: quantityToOrder
        });

        // Perform AJAX to update stock
        $.ajax({
            url: '/Management/Product/UpdateStock',
            method: 'POST',
            data: {
                productId: productId,
                quantityToOrder: quantityToOrder
            },
            success: function (response) {
                // Update the quantity after successful request
                if (response.success) {
                    row.find('.available-quantity').text(response.updatedStock);
                    alert('Stock updated successfully!');
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred while updating stock.');
            }
        });
    });
});
