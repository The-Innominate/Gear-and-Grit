public interface IObserver
{
	//Cant be called just update because Unity has a function called update
	void UpdateWhenNotified();
}
