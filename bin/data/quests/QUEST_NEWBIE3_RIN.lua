QUEST_NEWBIE3_RIN = {
	title = 'IDS_PROPQUEST_INC_002728',
	character = 'MaFl_Newbie',
	end_character = 'MaFl_Newbie',
	start_requirements = {
		min_level = 75,
		max_level = 129,
		job = { 'JOB_RINGMASTER', 'JOB_RINGMASTER_MASTER', 'JOB_RINGMASTER_HERO' },
		previous_quest = '',
	},
	rewards = {
		gold = 0,
		exp = 0,
		items = {
			{ id = 'II_SYS_SYS_EVE_BXMRINGMASTER90', quantity = 1, sex = 'Male' },
			{ id = 'II_SYS_SYS_EVE_BXFRINGMASTER90', quantity = 1, sex = 'Female' },
		},
	},
	end_conditions = {
		items = {
			{ id = 'II_SYS_SYS_EVE_NEWBIE03', quantity = 1, sex = 'Any', remove = true },
		},
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_INC_002729',
		},
		begin_yes = {
			'IDS_PROPQUEST_INC_002730',
		},
		begin_no = {
			'IDS_PROPQUEST_INC_002731',
		},
		completed = {
			'IDS_PROPQUEST_INC_002732',
		},
		not_finished = {
			'IDS_PROPQUEST_INC_002733',
		},
	}
}
