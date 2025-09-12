import React, { useState, useEffect } from 'react';
import { DragonBallCharacter, CreateCharacterRequest } from '../types/DragonBallCharacter';
import './CharacterForm.css';

interface CharacterFormProps {
  character?: DragonBallCharacter | null;
  onSubmit: (character: CreateCharacterRequest) => void;
  onCancel: () => void;
}

const CharacterForm: React.FC<CharacterFormProps> = ({ character, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState<CreateCharacterRequest>({
    name: '',
    race: '',
    planet: '',
    transformation: '',
    technique: ''
  });

  useEffect(() => {
    if (character) {
      setFormData({
        name: character.name,
        race: character.race,
        planet: character.planet,
        transformation: character.transformation,
        technique: character.technique
      });
    } else {
      setFormData({
        name: '',
        race: '',
        planet: '',
        transformation: '',
        technique: ''
      });
    }
  }, [character]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.name && formData.race && formData.planet && formData.transformation && formData.technique) {
      onSubmit(formData);
    }
  };

  return (
    <div className="character-form-overlay">
      <div className="character-form">
        <h2>{character ? 'Edit Character' : 'Add New Character'}</h2>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="name">Name:</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              placeholder="Character name"
            />
          </div>

          <div className="form-group">
            <label htmlFor="race">Race:</label>
            <input
              type="text"
              id="race"
              name="race"
              value={formData.race}
              onChange={handleChange}
              required
              placeholder="e.g., Saiyan, Namekian, Human"
            />
          </div>

          <div className="form-group">
            <label htmlFor="planet">Planet:</label>
            <input
              type="text"
              id="planet"
              name="planet"
              value={formData.planet}
              onChange={handleChange}
              required
              placeholder="Home planet"
            />
          </div>

          <div className="form-group">
            <label htmlFor="transformation">Transformation:</label>
            <input
              type="text"
              id="transformation"
              name="transformation"
              value={formData.transformation}
              onChange={handleChange}
              required
              placeholder="Ultimate transformation"
            />
          </div>

          <div className="form-group">
            <label htmlFor="technique">Technique:</label>
            <input
              type="text"
              id="technique"
              name="technique"
              value={formData.technique}
              onChange={handleChange}
              required
              placeholder="Signature technique"
            />
          </div>

          <div className="form-actions">
            <button type="button" className="btn-cancel" onClick={onCancel}>
              Cancel
            </button>
            <button type="submit" className="btn-save">
              {character ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CharacterForm;