//  Sometimes partial functions happen by accident

let divide x y = x / y

//  This looks like a total function, but what happens when
//  you pass in zero for the denominator?

//  We get a runtime exception.