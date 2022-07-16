// babel plugin core, used to implement the core transformation engine
const babel = require("babel-core");
// Can achieve type conversion, generate AST parts
const types = require("babel-types");

let code = `let sum = (a, b) => a + b;`;

// Create a visitor to handle a specific type of node
let visitor = {
  // This plugin is called when the grammar is traversed and the variable name is equal to the function name of our plugin.
  ArrowFunctionExpression(path) {
    // console.log(path)
    let params = path.node.params;
    let body = types.blockStatement([types.returnStatement(path.node.body)]);
    let fn = types.functionExpression(null, params, body, false, false);
    path.replaceWith(fn);
  },
};
// Create arrow processing plugin
let arrowPlugin = { visitor };
// babel internally converts the code to AST first, then traverses it, traverses to type equal to ArrowFunctionExpression, and passes it to the plugin arrayPlugin
// Pass the plugin into babel and get the result
let result = babel.transform(code, {
  plugins: [arrowPlugin],
});
console.log("result->", result.code);
/* 
result let sum = function (a, b) {
  return a + b;
};
*/
