export class IdGenerator
{
    private _LastGeneratedId: number = 0;

    public GenerateUniqueId() : number
    {
        // add one to the _LastGeneratedId and return it
        return ++this._LastGeneratedId;
    }
}