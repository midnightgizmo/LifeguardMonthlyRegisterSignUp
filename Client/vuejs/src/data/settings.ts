
export class WebSiteLocationData
{
    

    public static get getCookieName() : string
    {
        return "jwtCookie";
    }

    public static get getAdminCookieName() : string
    {
        return "jwtAdminCookie";
    }
}

interface enviromentVariables
{
    VUE_APP_RootAppLocation : string;
    VUE_APP_apiPostBackURL : string
}