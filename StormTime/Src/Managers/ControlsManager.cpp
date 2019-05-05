#include "ControlsManager.h"

namespace Managers {
	ControlsManager* ControlsManager::_instance = nullptr;

	ControlsManager::ControlsManager()
	{
		for (int i = sf::Keyboard::Key::A; i != sf::Keyboard::Key::Pause; i++)
		{
			KeyStatusInfo keyStatusInfo{};
			keyStatusInfo.updateOccurred = true;
			keyStatusInfo.keyState = Released;

			_keyStates.push_back(keyStatusInfo);
		}
	}

	ControlsManager::~ControlsManager()
	{
	}

	ControlsManager* ControlsManager::Instance()
	{
		if (_instance == nullptr)
			_instance = new ControlsManager();

		return _instance;
	}

	ControlsManager::KeyState ControlsManager::getKeyState(int keyCode)
	{
		return _keyStates[keyCode].keyState;
	}

	void ControlsManager::handleKeyPress(int keyCode)
	{
		_keyStates[keyCode].keyState = JustPressed;
		_keyStates[keyCode].updateOccurred = false;
	}

	void ControlsManager::handleKeyReleased(int keyCode)
	{
		_keyStates[keyCode].keyState = JustReleased;
		_keyStates[keyCode].updateOccurred = false;
	}

	void ControlsManager::update()
	{
		for (size_t i = 0; i < _keyStates.size(); i++)
		{
			const KeyState currentKeyState = _keyStates[i].keyState;
			const bool updateOccurred = _keyStates[i].updateOccurred;

			if (currentKeyState == JustReleased && !updateOccurred)
			{
				_keyStates[i].keyState = Released;
				_keyStates[i].updateOccurred = true;
			}
			else if (currentKeyState == JustPressed && !updateOccurred)
			{
				_keyStates[i].keyState = Pressed;
				_keyStates[i].updateOccurred = true;
			}
		}
	}
}