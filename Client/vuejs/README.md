# Vue.js Client side part of lifeguard monthly register sign up 

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

# Set server and client URLs

Within the root of the project is a `.env` file. When opening this file up we can see two Variables that need to be set.

## VUE_APP_RootAppLocation

'VUE_APP_RootAppLocation' is the folder the vuejs project is running within on the domain.

If you are running the vuejs project at the root of the domain e.g. http://localhost.com/ then set the `VUE_APP_RootAppLocation` like below
```
VUE_APP_RootAppLocation=/
```

If you are running the vuejs project in one more more sub folders, e.g. http://localhost.com/apps/myapp/ then set the `VUE_APP_RootAppLocation` like below
like below
```
VUE_APP_RootAppLocation=/apps/myapp/
```

## VUE_APP_apiPostBackURL

This is the full url location to where the server side API calls are made. For example if the Server code is located at http://localhost.com/server/ Then the `VUE_APP_apiPostBackURL` will need to be set as below
```
VUE_APP_apiPostBackURL=http://localhost.com/server/API
```
> note that there is no forward slash at the end of the location








