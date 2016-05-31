function mynewdate() {
    var newdate = { stardate: 2013+'-'+06+'-'+01, enddate: 2014+'-'+12+'-'+31};
    return newdate;
}
function randomDate(){
var Y = 2000 + Math.round(Math.random() * 10);
var M = 1 + Math.round(Math.random() * 11);
var D = 1 + Math.round(Math.random() * 27);
return Y+'-'+M+'-'+D;
}