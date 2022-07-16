
module.exports = function ({types:t}) {
  return {
               // Transcode the import
      visitor:{
          ImportDeclaration(path, _ref = { opts: {} }) {
              const specifiers = path.node.specifiers;
              const source = path.node.source;
                               // Transcode only if libraryName is satisfied
              if  (_ref.opts.library == source.value && (!t.isImportDefaultSpecifier(specifiers[0]))) { //_ref.opts is the passed argument
                                       var declarations = specifiers.map((specifier) ​​=> { // traverse uniq extend flatten cloneDeep
                      return  t.ImportDeclaration ( //Create importImportDeclaration node
                          [t.importDefaultSpecifier(specifier.local)],
                          t.StringLiteral(`${source.value}/${specifier.local.name}`)
                      )
                  })
                  path.replaceWithMultiple(declarations)
              }
          }
      }
  };
}