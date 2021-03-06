var regexp = /([0-9\.]+)|([ \t]+)|([\r\n]+)|(\*)|(\/)|(\+)|(\-)/g;

var dictionary = ["Number", "Whitespace", "LineTerminator", "*", "/", "+", "-"];

function* tokenize(source) {
  var result = null;
  var lastIndex = 0;
  while (true) {
    lastIndex = regexp.lastIndex;
    result = regexp.exec(source);

    if (!result) break;

    if (regexp.lastIndex - lastIndex > result[0].length) break;

    let token = {
      type: null,
      value: null,
    };

    for (var i = 1; i <= dictionary.length; i++) {
      if (result[i]) token.type = dictionary[i - 1];
    }
    token.value = result[0];
    yield token;
  }
  yield {
    type: "EOF",
  };
}

let source = [];

for (let token of tokenize("10 * 25 / 2+5-1")) {
  if (token.type !== "Whitespace" && token.type !== "LineTerminator")
    source.push(token);
}

function Expression(tokens) {
  if (
    source[0].type === "AdditiveExpression" &&
    source[1] &&
    source[1].type === "EOF"
  ) {
    let node = {
      type: "Expression",
      children: [source.shift(), source.shift()],
    };
    source.unshift(node);
    return node;
  }
  AdditiveExpression(source);
  return Expression(source);
}

function AdditiveExpression(source) {
  if (source[0].type === "MultiplicativeExpresson") {
    let node = {
      type: "AdditiveExpression",
      children: [source[0]],
    };
    source[0] = node;
    return AdditiveExpression(source);
  }

  if (
    source[0].type === "AdditiveExpression" &&
    source[1] &&
    source[1].type === "+"
  ) {
    let node = {
      type: "AdditiveExpression",
      operator: "+",
      children: [],
    };
    node.children.push(source.shift());
    node.children.push(source.shift());

    MultiplicativeExpresson(source);
    node.children.push(source.shift());
    source.unshift(node);
    return AdditiveExpression(source);
  }

  if (
    source[0].type === "AdditiveExpression" &&
    source[1] &&
    source[1].type === "-"
  ) {
    let node = {
      type: "AdditiveExpression",
      operator: "-",
      children: [],
    };
    node.children.push(source.shift());
    node.children.push(source.shift());
    MultiplicativeExpresson(source);
    node.children.push(source.shift());
    source.unshift(node);
    return AdditiveExpression(source);
  }

  if (source[0].type === "AdditiveExpression") return source[0];

  MultiplicativeExpresson(source);
  return AdditiveExpression(source);
}

function MultiplicativeExpresson(source) {
  if (source[0].type === "Number") {
    let node = {
      type: "MultiplicativeExpresson",
      children: [source[0]],
    };
    source[0] = node;
    return MultiplicativeExpresson(source);
  }

  if (
    source[0].type === "MultiplicativeExpresson" &&
    source[1] &&
    source[1].type === "*"
  ) {
    let node = {
      type: "MultiplicativeExpresson",
      operator: "*",
      children: [],
    };
    node.children.push(source.shift());
    node.children.push(source.shift());
    node.children.push(source.shift());
    source.unshift(node);
    return MultiplicativeExpresson(source);
  }

  if (
    source[0].type === "MultiplicativeExpresson" &&
    source[1] &&
    source[1].type === "/"
  ) {
    let node = {
      type: "MultiplicativeExpresson",
      operator: "*",
      children: [],
    };
    node.children.push(source.shift());
    node.children.push(source.shift());
    node.children.push(source.shift());
    source.unshift(node);
    return MultiplicativeExpresson(source);
  }

 
  if (source[0].type === "MultiplicativeExpresson") return source[0];

  return MultiplicativeExpresson(source);
}

console.log(JSON.stringify(Expression(source)));
