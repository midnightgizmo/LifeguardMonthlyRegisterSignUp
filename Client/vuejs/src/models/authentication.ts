

export class loginAuthentication
{
    
    private _isLoggedIn : boolean = false;
    public get isLoggedIn() : boolean {
        return this._isLoggedIn;
    }
    public set isLoggedIn(v : boolean) {
        this._isLoggedIn = v;
    }

    
    private _errorMessage : string = '';
    // if isLoggedIn is false, errorMessage will contain
    // information about why they are not logged in.
    public get errorMessage() : string {
        return this._errorMessage;
    }
    public set errorMessage(v : string) {
        this._errorMessage = v;
    }
    
    private _jwt: string = '';
    // the json web token used to authenticate the user
    public get jwt(): string
    {
        return this._jwt;
    }
    public set jwt(v: string)
    {
        this._jwt = v;
    }
    
}