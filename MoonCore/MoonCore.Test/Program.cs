using MoonCore.Extended.Helpers;
using MoonCore.Extended.Models;
using MoonCore.Helpers;

var secret = "sasaasasasasassasaasassssssssssssssssss";
var jwt =
    "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwieCI6InkifQ.n2Pl9K3rvTx-BtaljY4LMsrVidOyxKFLncD0GG6Ct_RNiThkqwpAakrstbMbSTsLdUTmidZ3SgDu44xxAOu_pQ";

if (JwtHelper.VerifySignature(secret, jwt))
    Console.WriteLine("Valid");
else
    Console.WriteLine("Not valid");

var x = JwtHelper.Decode(
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjYxNjIzOTAyMn0.VuEiPzAwyz7RPpejG10djC5FZXnp0i5FdfQCClZ9xZ4"
);

x.CheckTimestampClaims();