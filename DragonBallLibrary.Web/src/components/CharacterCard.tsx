import React from 'react';
import { DragonBallCharacter } from '../types/DragonBallCharacter';
import './CharacterCard.css';

interface CharacterCardProps {
  character: DragonBallCharacter;
  onEdit: (character: DragonBallCharacter) => void;
  onDelete: (id: number) => void;
}

const CharacterCard: React.FC<CharacterCardProps> = ({ character, onEdit, onDelete }) => {
  const handleImageError = (e: React.SyntheticEvent<HTMLImageElement>) => {
    // If the image fails to load, hide it
    e.currentTarget.style.display = 'none';
  };

  return (
    <div className="character-card">
      {character.imageUrl && (
        <div className="character-image">
          <img 
            src={character.imageUrl} 
            alt={character.name}
            onError={handleImageError}
          />
        </div>
      )}
      
      <div className="character-header">
        <h3 className="character-name">{character.name}</h3>
        <span className="character-race">{character.race}</span>
      </div>
      
      <div className="character-details">
        <div className="character-info">
          <div className="info-item">
            <span className="info-label">Planet:</span>
            <span className="info-value">{character.planet}</span>
          </div>
          <div className="info-item">
            <span className="info-label">Transformation:</span>
            <span className="info-value">{character.transformation}</span>
          </div>
          <div className="info-item">
            <span className="info-label">Technique:</span>
            <span className="info-value">{character.technique}</span>
          </div>
        </div>
      </div>
      
      <div className="character-actions">
        <button 
          className="btn-edit" 
          onClick={() => onEdit(character)}
        >
          Edit
        </button>
        <button 
          className="btn-delete" 
          onClick={() => onDelete(character.id)}
        >
          Delete
        </button>
      </div>
    </div>
  );
};

export default CharacterCard;