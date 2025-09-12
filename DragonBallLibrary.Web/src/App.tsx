import React, { useState, useEffect } from 'react';
import './App.css';
import { DragonBallCharacter, CreateCharacterRequest } from './types/DragonBallCharacter';
import { CharacterService } from './services/characterService';
import CharacterCard from './components/CharacterCard';
import CharacterForm from './components/CharacterForm';

function App() {
  const [characters, setCharacters] = useState<DragonBallCharacter[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingCharacter, setEditingCharacter] = useState<DragonBallCharacter | null>(null);

  useEffect(() => {
    loadCharacters();
  }, []);

  const loadCharacters = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await CharacterService.getAllCharacters();
      setCharacters(data);
    } catch (err) {
      setError('Failed to load characters. Make sure the API server is running.');
      console.error('Error loading characters:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCharacter = async (characterData: CreateCharacterRequest) => {
    try {
      const newCharacter = await CharacterService.createCharacter(characterData);
      setCharacters(prev => [...prev, newCharacter]);
      setShowForm(false);
    } catch (err) {
      setError('Failed to create character.');
      console.error('Error creating character:', err);
    }
  };

  const handleUpdateCharacter = async (characterData: CreateCharacterRequest) => {
    if (!editingCharacter) return;
    
    try {
      const updatedCharacter = await CharacterService.updateCharacter(editingCharacter.id, characterData);
      setCharacters(prev => prev.map(char => 
        char.id === editingCharacter.id ? updatedCharacter : char
      ));
      setEditingCharacter(null);
      setShowForm(false);
    } catch (err) {
      setError('Failed to update character.');
      console.error('Error updating character:', err);
    }
  };

  const handleDeleteCharacter = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this character?')) {
      return;
    }

    try {
      await CharacterService.deleteCharacter(id);
      setCharacters(prev => prev.filter(char => char.id !== id));
    } catch (err) {
      setError('Failed to delete character.');
      console.error('Error deleting character:', err);
    }
  };

  const handleEditCharacter = (character: DragonBallCharacter) => {
    setEditingCharacter(character);
    setShowForm(true);
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingCharacter(null);
  };

  const handleSubmitForm = (characterData: CreateCharacterRequest) => {
    if (editingCharacter) {
      handleUpdateCharacter(characterData);
    } else {
      handleCreateCharacter(characterData);
    }
  };

  if (loading) {
    return (
      <div className="app-loading">
        <div className="loading-spinner"></div>
        <p>Loading Dragon Ball characters...</p>
      </div>
    );
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="header-content">
          <h1 className="app-title">🐉 Dragon Ball Character Library</h1>
          <p className="app-subtitle">Manage your favorite Dragon Ball characters</p>
          <button 
            className="btn-add-character" 
            onClick={() => setShowForm(true)}
          >
            + Add New Character
          </button>
        </div>
      </header>

      {error && (
        <div className="error-message">
          <p>{error}</p>
          <button onClick={loadCharacters} className="btn-retry">
            Retry
          </button>
        </div>
      )}

      <main className="app-main">
        {characters.length === 0 && !error ? (
          <div className="empty-state">
            <h2>No characters found</h2>
            <p>Start building your Dragon Ball character library!</p>
            <button 
              className="btn-add-first" 
              onClick={() => setShowForm(true)}
            >
              Add First Character
            </button>
          </div>
        ) : (
          <div className="characters-grid">
            {characters.map(character => (
              <CharacterCard
                key={character.id}
                character={character}
                onEdit={handleEditCharacter}
                onDelete={handleDeleteCharacter}
              />
            ))}
          </div>
        )}
      </main>

      {showForm && (
        <CharacterForm
          character={editingCharacter}
          onSubmit={handleSubmitForm}
          onCancel={handleCloseForm}
        />
      )}
    </div>
  );
}

export default App;
