#include "AssetManager.h"

namespace Managers
{
	AssetManager::AssetManager()
	{
		Background = "resources/images/Background.png";

		CircleShape_1 = "resources/images/Circle_Shape_1.png";
		CircleShape_2 = "resources/images/Circle_Shape_2.png";
		CircleShape_3 = "resources/images/Circle_Shape_3.png";
		CircleShape_4 = "resources/images/Circle_Shape_4.png";

		SquareShape_1 = "resources/images/Square_Shape_1.png";
		SquareShape_2 = "resources/images/Square_Shape_2.png";
		SquareShape_3 = "resources/images/Square_Shape_3.png";
		SquareShape_4 = "resources/images/Square_Shape_4.png";
	}

	AssetManager::~AssetManager() = default;

	std::string AssetManager::texturePath(SharedTexturesManager::TextureType textureType)
	{
		switch (textureType)
		{
		case SharedTexturesManager::Background:
			return  Background;

		case SharedTexturesManager::Circle_1:
			return CircleShape_1;

		case SharedTexturesManager::Circle_2:
			return CircleShape_2;

		case SharedTexturesManager::Circle_3:
			return CircleShape_3;

		case SharedTexturesManager::Circle_4:
			return CircleShape_4;

		case SharedTexturesManager::Square_1:
			return SquareShape_1;

		case SharedTexturesManager::Square_2:
			return SquareShape_2;

		case SharedTexturesManager::Square_3:
			return SquareShape_3;

		case SharedTexturesManager::Square_4:
			return SquareShape_4;

		default:
			return  "";
		}
	}
}
