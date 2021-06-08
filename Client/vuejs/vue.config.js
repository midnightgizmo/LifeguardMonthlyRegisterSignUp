module.exports = {
    publicPath: process.env.VUE_APP_RootAppLocation,
    productionSourceMap: true,
    configureWebpack: config => {
      if (process.env.NODE_ENV === 'development') {
        config.devtool = 'source-map';
        config.output.devtoolModuleFilenameTemplate = info =>
          info.resourcePath.match(/\.vue$/) && !info.identifier.match(/type=script/)  // this is change ✨ 
            ? `webpack-generated:///${info.resourcePath}?${info.hash}`
            : `webpack-yourCode:///${info.resourcePath}`;
        
  
        config.output.devtoolFallbackModuleFilenameTemplate = 'webpack:///[resource-path]?[hash]';
      }
    }
  }

/*
module.exports = {
    productionSourceMap: true,
    configureWebpack: {
      devtool: 'source-map'
    }
  }*/