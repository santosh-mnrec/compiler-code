let babel = require("babel-core");
// Can achieve type judgment, generate AST node
let types = require("babel-types");
let code = `codes.map(code=>{return  code.toUpperCase()})`; //Conversion statement
//visitor can handle specific nodes
let visitor = {
  ArrowFunctionExpression(path) {
    //Define the node that needs to be converted, here intercept arrow function
    let params = path.node.params;
    let blockStatement = path.node.body;
    // Use babel-typesfunctionExpression method generates a new node
    let func = types.functionExpression(
      null,
      params,
      blockStatement,
      false,
      false
    );
    // Replace the node
    path.replaceWith(func); //
  },
};
// Convert code to ast
let result = babel.transform(code, {
  plugins: [{ visitor }],
});
console.log(result.code);
