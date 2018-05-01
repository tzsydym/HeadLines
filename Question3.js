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
    //override/add toSting and valueOf method for temp
    temp.toString = temp.valueOf = function() {
        return sum; 
    }
    return temp;
}
//the type of add(1)(2)(3) is function but with value of 6
//add(1)(2)(3)() will return a number with value 6
//in some context, like document.getElementById("p1").innerHTML = add(1)(2)(3)
//overrided toString() or valueOf() will be called and we can get the answer
