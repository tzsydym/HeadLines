function add(a) {
    var sum = 0;    
        sum = createMathOperation((augend, addend) => augend + addend, 0)(sum,a);        
    var temp = function(b) {    
        if(arguments.length===0){
            return sum;
        } else {
            sum = sum+ b;
            return temp;
        }
    }  
    temp.toString = temp.valueOf = function() {
        return sum; 
    }
    return temp;
}
