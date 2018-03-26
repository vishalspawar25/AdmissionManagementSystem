//To format currency
function formatCurrency(val) {
    return val.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
}