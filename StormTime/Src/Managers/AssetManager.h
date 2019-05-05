#pragma once

#include  <string>
#include "SharedTexturesManager.h"

namespace Managers {
	class AssetManager
	{
	private:
		AssetManager();

		static std::string Background;

		static std::string CircleShape_1;
		static std::string CircleShape_2;
		static std::string CircleShape_3;
		static std::string CircleShape_4;

		static std::string SquareShape_1;
		static std::string SquareShape_2;
		static std::string SquareShape_3;
		static std::string SquareShape_4;

	public:
		~AssetManager();

		static std::string texturePath(SharedTexturesManager::TextureType textureType);
	};
}
