using UnityEngine;

public interface IRidable
{
    int IndexInList {  get; set; }

    Transform GetAnimalTransform();
    Transform GetSeatTransform();



    bool IsHighlighted();
    
    void Highlight();
    void UnHighlight();
    

    void OnRide();
    void OnRelease();
}
