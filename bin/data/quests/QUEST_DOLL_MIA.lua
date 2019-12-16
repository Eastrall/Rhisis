QUEST_DOLL_MIA = {
	title = 'IDS_PROPQUEST_INC_001038',
	character = 'MaSa_Porgo',
	start_requirements = {
		min_level = 20,
		max_level = 30,
		job = { 'JOB_MERCENARY', 'JOB_ACROBAT', 'JOB_ASSIST', 'JOB_MAGICIAN' },
	},
	rewards = {
		gold = 0,
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_INC_001039',
			'IDS_PROPQUEST_INC_001040',
			'IDS_PROPQUEST_INC_001041',
			'IDS_PROPQUEST_INC_001042',
		},
		begin_yes = {
			'IDS_PROPQUEST_INC_001043',
		},
		begin_no = {
			'IDS_PROPQUEST_INC_001044',
		},
		completed = {
			'IDS_PROPQUEST_INC_001045',
		},
		not_finished = {
			'IDS_PROPQUEST_INC_001046',
		},
	}
}
