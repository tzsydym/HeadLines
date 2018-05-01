//please use the following functions to replace add function of loadsh.js
var curry = function(fn) {
    var _arg = [];
    return function temp() {
        if (arguments.length === 0) {
            return fn.apply(null, _arg);
        } else {
            _arg = _arg.concat([].slice.call(arguments));
            return temp;
        }
    }
}
const add = curry(function() {    
    var i=0,len = arguments.length,sum = 0;
    for (i; i<len; i+=1) {
        sum = createMathOperation((augend, addend) => augend + addend, 0)(sum,arguments[i])        
    }
    return sum
});
